using System;
using System.Threading;
using System.Threading.Tasks;

namespace WorkflowCore.Interface
{
    /// <remarks>
    /// The implementation of this interface will be responsible for
    /// providing a (distributed) queueing mechanism to manage in flight workflows    
    /// </remarks>
    public interface IQueueProvider : IBackgroundTask, IDisposable
    {
        /// <summary>
        /// Enqueues work to be processed by a host in the cluster
        /// </summary>
        /// <param name="id">identifier of item to queue</param>
        /// <param name="queue">Queue type</param>
        /// <returns></returns>
        Task QueueWork(string id, QueueType queue);

        /// <summary>
        /// Fetches the next work item from the front of the process queue.
        /// If the queue is empty, NULL is returned
        /// </summary>
        /// <returns></returns>
        Task<string> DequeueWork(QueueType queue, CancellationToken cancellationToken);

        bool IsDequeueBlocking { get; }
    }

    /// <summary>
    /// Supported queue types
    /// </summary>
    public enum QueueType
    {
        /// <summary>
        /// Queue type for workflows
        /// </summary>
        Workflow = 0, 
        /// <summary>
        /// Queue type for events
        /// </summary>
        Event = 1
    }
}
