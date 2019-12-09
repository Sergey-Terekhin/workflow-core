using System;
using System.Threading.Tasks;
using WorkflowCore.Models.LifeCycleEvents;

namespace WorkflowCore.Interface
{
    /// <summary>
    /// Hub which manages subscriptions on lifecycle events
    /// </summary>
    public interface ILifeCycleEventHub : IBackgroundTask
    {
        /// <summary>
        /// Publish new notification with lifecycle event to the underlying provider
        /// </summary>
        /// <param name="evt">event to publish</param>
        /// <returns></returns>
        Task PublishNotification(LifeCycleEvent evt);
        
        /// <summary>
        /// Subscribes on lifecycle events
        /// </summary>
        /// <param name="action">action to invoke when notification is received</param>
        void Subscribe(Action<LifeCycleEvent> action);
    }
}