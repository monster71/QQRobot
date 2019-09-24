﻿using Data.Pikachu;
using Data.Pikachu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateMsg.Services
{
    /// <summary>
    /// @auth : monster
    /// @since : 2019/9/24 11:30:21
    /// @source : 
    /// @des : 
    /// </summary>
    public class ConfigService
    {


        private PikachuDataContext _dbContext;

        public ConfigService(PikachuDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 获取所有配置
        /// </summary>
        /// <returns></returns>
        public IQueryable<ConfigInfo> GetAll()
        {
            return _dbContext.ConfigInfos.Where(u => u.Enable);
        }

        /// <summary>
        /// 移除配置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="msg"></param>
        public void RemoveKey(string key,out string msg)
        {
            var search =
                _dbContext.ConfigInfos.FirstOrDefault(u => u.Enable && u.Key.Equals(key));
            if (search != null)
            {
                search.Enable = false;
                _dbContext.SaveChanges();
            }

            msg = "删除成功";
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="input"></param>
        /// <param name="msg"></param>
        public void AddInfo(string input,out string msg)
        {
            var info = input.Split('|');
            if (info.Length == 3)
            {
                if (!string.IsNullOrWhiteSpace(info[0]))
                {
                    var config = new ConfigInfo()
                    {
                        CreateTime = DateTime.Now,
                        Key = info[0].Trim(),
                        Value = info[1],
                        Description = info[2],
                        Enable = true
                    };

                    var old = _dbContext.ConfigInfos.FirstOrDefault(u =>
                        u.Enable && u.Key.Equals(config.Key, StringComparison.CurrentCultureIgnoreCase));

                    if (old != null)
                    {
                        old.Value = config.Value;
                        old.UpdateTime = DateTime.Now;
                    }
                    else
                    {
                        config.UpdateTime = DateTime.Now;
                        config.CreateTime = DateTime.Now;
                        _dbContext.ConfigInfos.Add(config);
                    }

                    _dbContext.SaveChanges();

                    msg = "   添加成功！";
                }
                else
                {
                    msg = "   配置key不能为空！";
                }
            }
            else
            {
                msg = "   输入格式有误！";
            }
        }

    }
}
