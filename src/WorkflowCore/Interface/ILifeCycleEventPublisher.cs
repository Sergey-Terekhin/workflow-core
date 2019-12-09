using System;
using WorkflowCore.Models.LifeCycleEvents;

namespace WorkflowCore.Interface
{
    /// <summary>
    /// interface to publish lifecycle events
    /// </summary>
    public interface ILifeCycleEventPublisher : IBackgroundTask
    {
        /// <summary>
        /// Publish new notification with lifecycle event to the underlying provider
        /// </summary>
        /// <param name="evt">event to publish</param>
        void PublishNotification(LifeCycleEvent evt);
    }
}
