using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using RedLockNet;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace WorkflowCore.LockProviders.Redlock.Services
{
    public class RedlockProvider : IDistributedLockProvider, IDisposable
    {
        private readonly RedLockFactory _redlockFactory;
        private readonly TimeSpan _lockTimeout = TimeSpan.FromMinutes(10);
        private readonly List<IRedLock> _managedLocks = new List<IRedLock>();

        public RedlockProvider(params DnsEndPoint[] endpoints)
        {
            var redlockEndpoints = new List<RedLockEndPoint>();

            foreach (var ep in endpoints)
                redlockEndpoints.Add(ep);
            

            _redlockFactory = RedLockFactory.Create(redlockEndpoints);

        }

        public async Task<bool> AcquireLock(string id, CancellationToken token)
        {
            
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

        public Task Start()
        {
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _redlockFactory?.Dispose();
        }

    }
}