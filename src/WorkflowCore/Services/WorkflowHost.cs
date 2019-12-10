using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.Models.LifeCycleEvents;
// ReSharper disable InconsistentNaming

namespace WorkflowCore.Services
{
    /// <summary>
    /// Implementation of <see cref="IWorkflowHost"/>
    /// </summary>
    public class WorkflowHost : IWorkflowHost, IDisposable
    {
        private bool _shutdown = true;

        private readonly IEnumerable<IBackgroundTask> _backgroundTasks;
        private readonly IWorkflowController _workflowController;

        /// <inheritdoc />
        public event StepErrorEventHandler OnStepError;

        /// <inheritdoc />
        public event LifeCycleEventHandler OnLifeCycleEvent;

        // Public dependencies to allow for extension method access.
        private IPersistenceProvider PersistenceStore { get; }
        private IDistributedLockProvider LockProvider { get; }
        private IWorkflowRegistry Registry { get; }
        private IQueueProvider QueueProvider { get; }
        private ILogger Logger { get; }

        private readonly ILifeCycleEventHub _lifeCycleEventHub;

        /// <summary>
        /// ctor
        /// </summary>
        public WorkflowHost(
            IPersistenceProvider persistenceStore,
            IQueueProvider queueProvider,
            ILoggerFactory loggerFactory,
            IWorkflowRegistry registry,
            IDistributedLockProvider lockProvider, 
            IEnumerable<IBackgroundTask> backgroundTasks,
            IWorkflowController workflowController, 
            ILifeCycleEventHub lifeCycleEventHub)
        {
            PersistenceStore = persistenceStore;
            QueueProvider = queueProvider;
            Logger = loggerFactory.CreateLogger<WorkflowHost>();
            Registry = registry;
            LockProvider = lockProvider;
            _backgroundTasks = backgroundTasks;
            _workflowController = workflowController;
            _lifeCycleEventHub = lifeCycleEventHub;
            _lifeCycleEventHub.Subscribe(HandleLifeCycleEvent);
        }
        
        /// <inheritdoc />
        public Task<string> StartWorkflow(string workflowId, object data = null, string reference=null)
        {
            return _workflowController.StartWorkflow(workflowId, data, reference);
        }

        /// <inheritdoc />
        public Task<string> StartWorkflow(string workflowId, int? version, object data = null, string reference=null)
        {
            return _workflowController.StartWorkflow<object>(workflowId, version, data, reference);
        }

        /// <inheritdoc />
        public Task<string> StartWorkflow<TData>(string workflowId, TData data = null, string reference=null)
            where TData : class, new()
        {
            return _workflowController.StartWorkflow(workflowId, null, data, reference);
        }
        
        /// <inheritdoc />
        public Task<string> StartWorkflow<TData>(string workflowId, int? version, TData data = null, string reference=null)
            where TData : class, new()
        {
            return _workflowController.StartWorkflow(workflowId, version, data, reference);
        }

        /// <inheritdoc />
        public Task PublishEvent(string eventName, string eventKey, object eventData, DateTime? effectiveDate = null)
        {
            return _workflowController.PublishEvent(eventName, eventKey, eventData, effectiveDate);
        }

        /// <inheritdoc />
        public async Task Start()
        {
            _shutdown = false;
            PersistenceStore.EnsureStoreExists();
            
            await Task.WhenAll(
                QueueProvider.Start(),
                LockProvider.Start(),
                _lifeCycleEventHub.Start());
            
            Logger.LogInformation(WellKnownLoggingEventIds.BackgroundTaskStart, "Starting background tasks");

            foreach (var task in _backgroundTasks)
            {
                Logger.LogInformation(WellKnownLoggingEventIds.BackgroundTaskStart, "Starting task {Task}", task.GetType());
                await task.Start();
            }
        }

        /// <inheritdoc />
        public async Task Stop()
        {
            _shutdown = true;

            Logger.LogInformation(WellKnownLoggingEventIds.BackgroundTaskStopping, "Stopping background tasks");
            foreach (var th in _backgroundTasks)
            {
                Logger.LogInformation(WellKnownLoggingEventIds.BackgroundTaskStopping, "Stopping task {Task}", th.GetType());
                await th.Stop();
            }

            Logger.LogInformation(WellKnownLoggingEventIds.BackgroundTaskStopped, "Worker tasks stopped");

            await Task.WhenAll(
                QueueProvider.Stop(),
                LockProvider.Stop(),
                _lifeCycleEventHub.Stop());
        }

        /// <inheritdoc />
        public void RegisterWorkflow<TWorkflow>()
            where TWorkflow : IWorkflow, new()
        {
            TWorkflow wf = new TWorkflow();
            Registry.RegisterWorkflow(wf);
        }

        /// <inheritdoc />
        public void RegisterWorkflow<TWorkflow, TData>()
            where TWorkflow : IWorkflow<TData>, new()
            where TData : new()
        {
            TWorkflow wf = new TWorkflow();
            Registry.RegisterWorkflow(wf);
        }

        /// <inheritdoc />
        public Task<bool> SuspendWorkflow(string workflowId)
        {
            return _workflowController.SuspendWorkflow(workflowId);
        }

        /// <inheritdoc />
        public Task<bool> ResumeWorkflow(string workflowId)
        {
            return _workflowController.ResumeWorkflow(workflowId);
        }

        /// <inheritdoc />
        public Task<bool> TerminateWorkflow(string workflowId)
        {
            return _workflowController.TerminateWorkflow(workflowId);
        }

        private void HandleLifeCycleEvent(LifeCycleEvent evt)
        {
            OnLifeCycleEvent?.Invoke(evt);
        }

        /// <inheritdoc />
        public void ReportStepError(WorkflowInstance workflow, WorkflowStep step, Exception exception)
        {
            OnStepError?.Invoke(workflow, step, exception);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_shutdown)
                Stop().Wait();
        }
    }
}
