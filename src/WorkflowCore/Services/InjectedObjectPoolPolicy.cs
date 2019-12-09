﻿using System;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.DependencyInjection;

namespace WorkflowCore.Services
{
    internal class InjectedObjectPoolPolicy<T> : IPooledObjectPolicy<T>
    {
        private readonly IServiceProvider _provider;

        public InjectedObjectPoolPolicy(IServiceProvider provider)
        {
            _provider = provider;
        }

        /// <inheritdoc />
        public T Create()
        {
            return _provider.GetService<T>();
        }

        /// <inheritdoc />
        public bool Return(T obj)
        {
            return true;
        }
    }
}
