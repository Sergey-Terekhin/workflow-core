using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace WorkflowCore.Services
{
    /// <summary>
    /// Single node in-memory implementation of IDistributedLockProvider
    /// </summary>
    public class SingleNodeLockProvider : IDistributedLockProvider
    {
        private readonly HashSet<string> _locks = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly object _lock = new object();

        /// <inheritdoc />
        public Task<bool> AcquireLock(string id, CancellationToken token)
        {
            lock (_lock)
            {
                if (_locks.Contains(id))
                    return Task.FromResult(false);

                _locks.Add(id);
                return Task.FromResult(true);
            }
        }

        /// <inheritdoc />
        public Task ReleaseLock(string id)
        {
            lock (_lock)
            {
                _locks.Remove(id);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task Start()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task Stop()
        {
            return Task.CompletedTask;
        }
    }
}