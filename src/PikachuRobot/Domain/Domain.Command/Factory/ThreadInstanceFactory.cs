﻿using System;
using System.Threading;

namespace Domain.Command.Factory
{
    /// <summary>
    /// @auth : monster
    /// @since : 2019/9/25 15:34:56
    /// @source : 
    /// @des : 一个线程共享一个
    /// </summary>
    public class ThreadInstanceFactory<T> where T : class
    {

        private static readonly ThreadLocal<T> threadLocal = new ThreadLocal<T>();

        public static T Get(Func<T> func)
        {
            return threadLocal.Value = threadLocal.Value ?? func();
        }

    }
}
