using System;
using JetBrains.Annotations;

namespace WorkflowCore.Models.LifeCycleEvents
{
    /// <summary>
    /// Event for completed workflow step
    /// </summary>
    [PublicAPI]
    public class StepCompleted : LifeCycleEvent
    {
        /// <summary>
        /// Identifier of execution pointer
        /// </summary>
        public string ExecutionPointerId { get; set; }
        /// <summary>
        /// Identifier of Workflow step
        /// </summary>
        public int StepId { get; set; }
        /// <summary>
        /// External identifier of Workflow step
        /// </summary>
        public string StepExternalId { get; set; }
    }
}