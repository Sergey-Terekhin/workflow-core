using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Services
{
    public class TransientMemoryPersistenceProvider : IPersistenceProvider
    {
        private readonly ISingletonMemoryProvider _innerService;

        public TransientMemoryPersistenceProvider(ISingletonMemoryProvider innerService)
        {
            _innerService = innerService;
        }

        /// <inheritdoc />
        public Task<string> CreateEvent(Event newEvent) => _innerService.CreateEvent(newEvent);

        /// <inheritdoc />
        public Task<string> CreateEventSubscription(EventSubscription subscription) => _innerService.CreateEventSubscription(subscription);

        /// <inheritdoc />
        public Task<string> CreateNewWorkflow(WorkflowInstance workflow) => _innerService.CreateNewWorkflow(workflow);

        /// <inheritdoc />
        public void EnsureStoreExists() => _innerService.EnsureStoreExists();

        /// <inheritdoc />
        public Task<Event> GetEvent(string id) => _innerService.GetEvent(id);

        /// <inheritdoc />
        public Task<IEnumerable<string>> GetEvents(string eventName, string eventKey, DateTime asOf) => _innerService.GetEvents(eventName, eventKey, asOf);
        
        /// <inheritdoc />
        public Task RemoveEventsByKey(string eventKey)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<IEnumerable<string>> GetRunnableEvents(DateTime asAt) => _innerService.GetRunnableEvents(asAt);

        /// <inheritdoc />
        public Task<IEnumerable<string>> GetRunnableInstances(DateTime asAt) => _innerService.GetRunnableInstances(asAt);

        /// <inheritdoc />
        public Task<IEnumerable<EventSubscription>> GetSubcriptions(string eventName, string eventKey, DateTime asOf) => _innerService.GetSubcriptions(eventName, eventKey, asOf);

        /// <inheritdoc />
        public Task<WorkflowInstance> GetWorkflowInstance(string id) => _innerService.GetWorkflowInstance(id);

        /// <inheritdoc />
        public Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(IEnumerable<string> ids) => _innerService.GetWorkflowInstances(ids);

        /// <inheritdoc />
        public Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(WorkflowStatus? status, string type, DateTime? createdFrom, DateTime? createdTo, int skip, int take) => _innerService.GetWorkflowInstances(status, type, createdFrom, createdTo, skip, take);

        /// <inheritdoc />
        public Task MarkEventProcessed(string id) => _innerService.MarkEventProcessed(id);

        /// <inheritdoc />
        public Task MarkEventUnprocessed(string id) => _innerService.MarkEventUnprocessed(id);

        /// <inheritdoc />
        public Task PersistErrors(IEnumerable<ExecutionError> errors) => _innerService.PersistErrors(errors);

        /// <inheritdoc />
        public Task PersistWorkflow(WorkflowInstance workflow) => _innerService.PersistWorkflow(workflow);

        /// <inheritdoc />
        public Task TerminateSubscription(string eventSubscriptionId) => _innerService.TerminateSubscription(eventSubscriptionId);
    }
}
