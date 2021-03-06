﻿using IServiceSupply;
using Newtonsoft.Json;
using NLog;
using Services.PikachuSystem;
using Services.Utils;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Command.CusConst;

namespace GenerateMsg.GroupMsg
{
    /// <summary>
    /// @auth : monster
    /// @since : 2019/9/26 16:20:57
    /// @source : 
    /// @des : 成语接龙
    /// </summary>
    public class IdiomsSolitaireDeal : IGenerateGroupMsgDeal
    {
        private readonly IDatabase _database;
        private readonly Random _random = new Random();

        public IdiomsSolitaireDeal(IdiomsService idiomsService, IDatabase database,
            GroupActivityService activityLogService)
        {
            IdiomsService = idiomsService;
            this._database = database;
            ActivityLogService = activityLogService;
        }

        private IdiomsService IdiomsService { get; }
        private GroupActivityService ActivityLogService { get; }

        public async Task<GroupRes> Run(string msg, string account, string groupNo, Lazy<string> getLoginAccount)
        {
            if ("成语接龙".Equals(msg))
            {
                var count = await IdiomsService.GetCountAsync();

                var randIndex = _random.Next(count);

                var info = await IdiomsService.GetInfoAsync(randIndex + 1);

                // 写入活动缓存
                await _database.StringSetAsync(CacheConst.GetGroupActivityKey(groupNo), CacheConst.IdiomsSolitaire,
                    RuleConst.GroupActivityExpiry);

                // 缓存成语id
                await _database.StringSetAsync(CacheConst.GetIdiomsKey(groupNo), info.Id,
                    RuleConst.GroupActivityExpiry);

                // 缓存尝试次数
                await _database.StringSetAsync(CacheConst.GetIdiomsTryCountKey(groupNo), 0,
                    RuleConst.GroupActivityExpiry);

                // 添加活动日志
                var logId = await ActivityLogService.OpenActivityAsync(groupNo,
                    Data.Pikachu.Menu.ActivityTypes.IdiomsSolitaire, DateTime.Now.Add(RuleConst.GroupActivityExpiry));

                // 缓存日志key
                await _database.StringSetAsync(CacheConst.GetActivityLogKey(groupNo), logId,
                    RuleConst.GroupActivityExpiry);

                CreateCloseTime(logId, groupNo, getLoginAccount.Value);

                return $@"
>>>>>>>>>全员成语接龙已开启<<<<<<<<<<<<
    当前成语:{info.Word}
    尾拼:{info.LastSpell}
    成语解析:{info.Explanation}
";
            }

            return null;
        }

        /// <summary>
        /// 创建一个定时关闭活动的定时器
        /// </summary>
        private void CreateCloseTime(int logId, string groupNo, string loginQq)
        {
            // 使用线程池开启工作项替代timer
            ThreadPool.QueueUserWorkItem((state =>
            {
                Thread.Sleep(RuleConst.GroupActivityExpiry);

                if (ActivityLogService.CloseActivity(logId, "活动结束，自动关闭!", out var log) > 0)
                {
                    _database.ListLeftPush(CacheConst.GetGroupMsgListKey(groupNo),
                        JsonConvert.SerializeObject(new GroupItemRes()
                        {
                            Msg = $@"
>>>>>>>>>成语接龙已结束<<<<<<<<<<<<
    成功次数:{log.SuccessCount.ToString()}
    失败次数:{log.FailureCount.ToString()}
希望大家再接再厉！
"
                        }));
                }
            }));
        }
    }
}