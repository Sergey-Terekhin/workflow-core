using System;
using JetBrains.Annotations;

namespace WorkflowCore.Models.LifeCycleEvents
{
    /// <summary>
    /// Event for workflow moved to error state
    /// </summary>
    [PublicAPI]
    // ReSharper disable once InconsistentNaming
    public class WorkflowError : LifeCycleEvent
    {
        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Execution pointer identifier
        /// </summary>
        public string ExecutionPointerId { get; set; }

        /// <summary>
        /// Step identifier which execution was reason for error
        /// </summary>
        public int StepId { get; set; }
    }
}
