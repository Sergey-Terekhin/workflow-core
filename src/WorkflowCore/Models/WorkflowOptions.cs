using Microsoft.Extensions.DependencyInjection;
using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Services;

// ReSharper disable InconsistentNaming

namespace WorkflowCore.Models
{
    /// <summary>
    /// Global workflow options
    /// </summary>
    [PublicAPI]
    public class WorkflowOptions
    {
        internal Func<IServiceProvider, IPersistenceProvider> PersistenceFactory { get; private set; }
        internal Func<IServiceProvider, IQueueProvider> QueueFactory { get; private set; }
        internal Func<IServiceProvider, IDistributedLockProvider> LockFactory { get; private set; }
        internal Func<IServiceProvider, ILifeCycleEventHub> EventHubFactory { get; private set; }
        internal TimeSpan PollInterval { get; private set; }
        internal TimeSpan IdleTime { get; private set; }
        internal TimeSpan ErrorRetryInterval { get; private set; }
        internal int MaxConcurrentWorkflows { get; private set; } = Math.Max(Environment.ProcessorCount, 2);

        /// <summary>
        /// Container to register services. Used by extension methods
        /// </summary>
        public IServiceCollection Services { get; private set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="services">Container to register services. Used by extension methods</param>
        public WorkflowOptions(IServiceCollection services)
        {
            Services = services;
            PollInterval = TimeSpan.FromSeconds(10);
            IdleTime = TimeSpan.FromMilliseconds(100);
            ErrorRetryInterval = TimeSpan.FromSeconds(60);

            QueueFactory = sp => new SingleNodeQueueProvider();
            LockFactory = sp => new SingleNodeLockProvider();
            PersistenceFactory = sp => sp.GetService<ISingletonMemoryProvider>();
            EventHubFactory = sp => new SingleNodeEventHub(sp.GetService<ILoggerFactory>());
        }

        /// <summary>
        /// Specify implementation of <see cref="IPersistenceProvider"/>
        /// </summary>
        /// <param name="factory">Function to resolve instance of <see cref="IPersistenceProvider"/></param>
        public void UsePersistence(Func<IServiceProvider, IPersistenceProvider> factory)
            => PersistenceFactory = factory;

        /// <summary>
        /// Specify implementation of <see cref="IDistributedLockProvider"/>
        /// </summary>
        /// <param name="factory">Function to resolve instance of <see cref="IDistributedLockProvider"/></param>
        public void UseDistributedLockManager(Func<IServiceProvider, IDistributedLockProvider> factory) =>
            LockFactory = factory;

        /// <summary>
        /// Specify implementation of <see cref="IQueueProvider"/>
        /// </summary>
        /// <param name="factory">Function to resolve instance of <see cref="IQueueProvider"/></param>
        public void UseQueueProvider(Func<IServiceProvider, IQueueProvider> factory)
            => QueueFactory = factory;

        /// <summary>
        /// Specify implementation of <see cref="ILifeCycleEventHub"/>
        /// </summary>
        /// <param name="factory">Function to resolve instance of <see cref="ILifeCycleEventHub"/></param>
        public void UseEventHub(Func<IServiceProvider, ILifeCycleEventHub> factory)
            => EventHubFactory = factory;

        /// <summary>
        /// Specify poll interval to get new workflow instances. Default value is 10 seconds
        /// </summary>
        /// <param name="interval">Interval value</param>
        public void UsePollInterval(TimeSpan interval)
        {
            PollInterval = interval;
        }

        /// <summary>
        /// Specify interval to retry operation after error. Default value is 60 seconds
        /// </summary>
        /// <param name="interval">Interval value</param>
        public void UseErrorRetryInterval(TimeSpan interval)
        {
            ErrorRetryInterval = interval;
        }

        /// <summary>
        /// Specify count of workflow instances which can be executed concurrently. Default value is Max(ProcessorCount, 2)
        /// </summary>
        /// <param name="maxConcurrentWorkflows">Concurrency level</param>
        public void UseMaxConcurrentWorkflows(int maxConcurrentWorkflows)
        {
            MaxConcurrentWorkflows = maxConcurrentWorkflows;
        }
    }
}