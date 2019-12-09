using System;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

namespace WorkflowCore.Models.LifeCycleEvents
{
    /// <summary>
    /// Base class for events which reflect workflow transition to specific state
    /// </summary>
    [PublicAPI]
    public abstract class LifeCycleEvent
    {
        /// <summary>
        /// Timestamp when event was create
        /// </summary>
        public DateTime EventTimeUtc { get; set; }

        /// <summary>
        /// Identifier of workflow instance
        /// </summary>
        public string WorkflowInstanceId { get; set; }

        /// <summary>
        /// Identifier of workflow definition (scheme)
        /// </summary>
        public string WorkflowDefinitionId { get; set; }

        /// <summary>
        /// Workflow version
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Workflow reference
        /// </summary>
        public string Reference { get; set; }
    }
}
