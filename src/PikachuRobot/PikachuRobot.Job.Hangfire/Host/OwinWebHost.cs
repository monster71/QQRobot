﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Owin.Hosting;
using NLog;

namespace PikachuRobot.Job.Hangfire.Host
{
    /// <summary>
    /// @auth : monster
    /// @since : 2019/10/10 15:30:49
    /// @source : 
    /// @des : 
    /// </summary>
    public class OwinWebHost : IWebHost
    {

        private static readonly Logger Logger = LogManager.GetLogger(nameof(OwinWebHost));
        private static volatile int _state;

        // 保存Web服务的实例
        private static IDisposable _webhost;

        public Task StartAsync(string baseUrl, ILifetimeScope lifetimeScope)
        {
            if (_state == 0)
            {
                if (Interlocked.Exchange(ref _state, 1) == 0)
                {
                    try
                    {
                        _webhost = WebApp.Start(baseUrl, app => new StartUp().Configuration(app, lifetimeScope));
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                        _state = 0;
                    }
                }
            }
            
            return Task.FromResult(0);
        }

        public Task StopAsync()
        {
            if (_state == 1 && _webhost != null)
            {
                if (Interlocked.Exchange(ref _state, 0) == 1)
                {
                    _webhost.Dispose();
                }
            }
            _webhost?.Dispose();
            return Task.FromResult(0);
        }

        public bool IsOpen()
        {
            return _state == 1;
        }
    }
}
