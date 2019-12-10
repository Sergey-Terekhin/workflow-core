using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

// ReSharper disable CheckNamespace

namespace WorkflowCore.Services
{
    public interface ISingletonMemoryProvider : IPersistenceProvider
    {
    }

    /// <summary>
    /// In-memory implementation of IPersistenceProvider for demo and testing purposes
    /// </summary>
    public class MemoryPersistenceProvider : ISingletonMemoryProvider
    {
        private readonly List<WorkflowInstance> _instances = new List<WorkflowInstance>();
        private readonly List<EventSubscription> _subscriptions = new List<EventSubscription>();
        private readonly List<Event> _events = new List<Event>();

        /// <inheritdoc />
        public Task<string> CreateNewWorkflow(WorkflowInstance workflow)
        {
            lock (_instances)
            {
                workflow.Id = Guid.NewGuid().ToString();
                _instances.Add(workflow);
                return Task.FromResult(workflow.Id);
            }
        }

        /// <inheritdoc />
        public Task PersistWorkflow(WorkflowInstance workflow)
        {
            lock (_instances)
            {
                var existing = _instances.First(x => x.Id == workflow.Id);
                _instances.Remove(existing);
                _instances.Add(workflow);
            }
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<IEnumerable<string>> GetRunnableInstances(DateTime asAt)
        {
            lock (_instances)
            {
                var now = asAt.ToUniversalTime().Ticks;
                return Task.FromResult<IEnumerable<string>>(_instances
                    .Where(x => x.NextExecution.HasValue && x.NextExecution <= now)
                    .Select(x => x.Id)
                    .ToList());
            }
        }

        /// <inheritdoc />
        public Task<WorkflowInstance> GetWorkflowInstance(string id)
        {
            lock (_instances)
            {
                return Task.FromResult(_instances.First(x => x.Id == id));
            }
        }

        /// <inheritdoc />
        public Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                return Task.FromResult<IEnumerable<WorkflowInstance>> (new List<WorkflowInstance>());
            }

            lock (_instances)
            {
                return Task.FromResult(_instances.Where(x => ids.Contains(x.Id)));
            }
        }

        /// <inheritdoc />
        public Task<string> CreateEventSubscription(EventSubscription subscription)
        {
            lock (_subscriptions)
            {
                subscription.Id = Guid.NewGuid().ToString();
                _subscriptions.Add(subscription);
                return Task.FromResult(subscription.Id);
            }
        }

        /// <inheritdoc />
        public Task<IEnumerable<EventSubscription>> GetSubscriptions(string eventName, string eventKey,
            DateTime asOf)
        {
            lock (_subscriptions)
            {
                return Task.FromResult(_subscriptions
                    .Where(x => x.EventName == eventName && x.EventKey == eventKey && x.SubscribeAsOf <= asOf));
            }
        }

        /// <inheritdoc />
        public Task TerminateSubscription(string eventSubscriptionId)
        {
            lock (_subscriptions)
            {
                var sub = _subscriptions.Single(x => x.Id == eventSubscriptionId);
                _subscriptions.Remove(sub);
            }
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public void EnsureStoreExists()
        {
        }

        /// <inheritdoc />
        public Task<string> CreateEvent(Event newEvent)
        {
            lock (_events)
            {
                newEvent.Id = Guid.NewGuid().ToString();
                _events.Add(newEvent);
                return Task.FromResult(newEvent.Id);
            }
        }

        /// <inheritdoc />
        public Task MarkEventProcessed(string id)
        {
            lock (_events)
            {
                var evt = _events.FirstOrDefault(x => x.Id == id);
                if (evt != null)
                    evt.IsProcessed = true;
            }
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<IEnumerable<string>> GetRunnableEvents(DateTime asAt)
        {
            lock (_events)
            {
                return Task.FromResult<IEnumerable<string>>(_events
                    .Where(x => !x.IsProcessed)
                    .Where(x => x.EventTime <= asAt.ToUniversalTime())
                    .Select(x => x.Id)
                    .ToList());
            }
        }

        /// <inheritdoc />
        public Task<Event> GetEvent(string id)
        {
            lock (_events)
            {
                return Task.FromResult(_events.FirstOrDefault(x => x.Id == id));
            }
        }

        /// <inheritdoc />
        public Task<IEnumerable<string>> GetEvents(string eventName, string eventKey, DateTime asOf)
        {
            lock (_events)
            {
                return Task.FromResult<IEnumerable<string>>(_events
                    .Where(x => x.EventName == eventName && x.EventKey == eventKey)
                    .Where(x => x.EventTime >= asOf)
                    .Select(x => x.Id)
                    .ToList());
            }
        }

        /// <inheritdoc />
        public Task MarkEventUnprocessed(string id)
        {
            lock (_events)
            {
                var evt = _events.FirstOrDefault(x => x.Id == id);
                if (evt != null)
                {
                    evt.IsProcessed = false;
                }
            }
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task PersistErrors(IEnumerable<ExecutionError> errors) => Task.CompletedTask;
    }

}