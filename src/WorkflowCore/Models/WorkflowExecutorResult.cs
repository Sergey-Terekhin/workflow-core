using System.Collections.Generic;
// ReSharper disable InconsistentNaming

namespace WorkflowCore.Models
{
    /// <summary>
    /// Global execution result for workflow instance 
    /// </summary>
    public class WorkflowExecutorResult
    {
        /// <summary>
        /// List of subscriptions
        /// </summary>
        public List<EventSubscription> Subscriptions { get; set; } = new List<EventSubscription>();
        /// <summary>
        /// List of execution errors
        /// </summary>
        public List<ExecutionError> Errors { get; set; } = new List<ExecutionError>();
    }
}
