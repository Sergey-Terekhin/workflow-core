using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using WorkflowCore.Interface;

namespace WorkflowCore.Providers.Redis.Services
{
    public class RedisLockProvider : IDistributedLockProvider
    {
        private readonly string _connectionString;        
        private IConnectionMultiplexer _multiplexer;
        private RedLockFactory _redlockFactory;
        private readonly TimeSpan _lockTimeout = TimeSpan.FromMinutes(1);
        private readonly List<IRedLock> _managedLocks = new List<IRedLock>();

        public RedisLockProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> AcquireLock(string id, CancellationToken token)
        {
            if (_redlockFactory == null)
                throw new InvalidOperationException();

            var redLock = await _redlockFactory.CreateLockAsync(id, _lockTimeout);

            if (redLock.IsAcquired)
            {
                lock (_managedLocks)
                {
                    _managedLocks.Add(redLock);
                }
                return true;
            }

            return false;
        }

        public Task ReleaseLock(string id)
        {
            if (_redlockFactory == null)
                throw new InvalidOperationException();

            lock (_managedLocks)
            {
                foreach (var redLock in _managedLocks)
                {
                    if (redLock.Resource == id)
                    {
                        redLock.Dispose();
                        _managedLocks.Remove(redLock);
                        break;
                    }
                }
            }

            return Task.CompletedTask;
        }

        public async Task Start()
        {
            _multiplexer = await ConnectionMultiplexer.ConnectAsync(_connectionString);           
            _redlockFactory = RedLockFactory.Create(new List<RedLockMultiplexer>() { new RedLockMultiplexer(_multiplexer) });
        }

        public async Task Stop()
        {
            _redlockFactory?.Dispose();
            await _multiplexer.CloseAsync();
            _multiplexer = null;
            
        }
    }
}
