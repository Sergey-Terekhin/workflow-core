using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowCore.Models;

namespace WorkflowCore.Interface
{
    /// <remarks>
    /// The implementation of this interface will be responsible for
    /// persisting running workflow instances to a durable store
    /// </remarks>
    public interface IPersistenceProvider
    {
        /// <summary>
        /// Save workflow instance in the storage
        /// </summary>
        /// <param name="workflow">Instance to save</param>
        /// <returns>Identifier of saved instance</returns>
        Task<string> CreateNewWorkflow(WorkflowInstance workflow);

        /// <summary>
        /// Updated existing workflow instance in the storage
        /// </summary>
        /// <param name="workflow">Instance to update</param>
        Task PersistWorkflow(WorkflowInstance workflow);

        /// <summary>
        /// Returns collection of workflow instance identifiers which execution should be started by the specified time
        /// </summary>
        /// <param name="asAt">Time to check</param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetRunnableInstances(DateTime asAt);

        /// <summary>
        /// Returns workflow instance by identifier
        /// </summary>
        /// <param name="id">Instance identifier</param>
        /// <returns></returns>
        Task<WorkflowInstance> GetWorkflowInstance(string id);

        /// <summary>
        /// Returns collection of workflow instances by identifiers
        /// </summary>
        /// <param name="ids">Collection of identifiers to return</param>
        /// <returns></returns>
        Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(IEnumerable<string> ids);

        /// <summary>
        /// Save subscription to the storage
        /// </summary>
        /// <param name="subscription">Subscription to save</param>
        /// <returns>Identifier of saved subscription</returns>
        Task<string> CreateEventSubscription(EventSubscription subscription);

        Task<IEnumerable<EventSubscription>> GetSubscriptions(string eventName, string eventKey, DateTime asOf);

        Task TerminateSubscription(string eventSubscriptionId);

        /// <summary>
        /// Save event to the storage
        /// </summary>
        /// <param name="newEvent">Event to save</param>
        /// <returns>Identifier of saved event</returns>
        Task<string> CreateEvent(Event newEvent);

        /// <summary>
        /// Returns event by identifier
        /// </summary>
        /// <param name="id">Event's identifier</param>
        /// <returns></returns>
        Task<Event> GetEvent(string id);

        Task<IEnumerable<string>> GetRunnableEvents(DateTime asAt);
        
        Task<IEnumerable<string>> GetEvents(string eventName, string eventKey, DateTime asOf);

        /// <summary>
        /// Mark event with specified identifier as processed
        /// </summary>
        /// <param name="id">Event's identifier</param>
        /// <returns></returns>
        Task MarkEventProcessed(string id);

        /// <summary>
        /// Mark event with specified identifier as unprocessed
        /// </summary>
        /// <param name="id">Event's identifier</param>
        /// <returns></returns>
        Task MarkEventUnprocessed(string id);

        /// <summary>
        /// Save list errors in the storage
        /// </summary>
        /// <param name="errors">List of errors to save</param>
        /// <returns></returns>
        Task PersistErrors(IEnumerable<ExecutionError> errors);

        /// <summary>
        /// Creates storage if it does not exist
        /// </summary>
        void EnsureStoreExists();
    }
}