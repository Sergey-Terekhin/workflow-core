using System;
using JetBrains.Annotations;


namespace WorkflowCore.Models
{
    /// <summary>
    /// Workflow definition (scheme)
    /// </summary>
    [PublicAPI]
    // ReSharper disable once InconsistentNaming
    public class WorkflowDefinition
    {
        /// <summary>
        /// Identifier of workflow definition (scheme). Must be unique in <see cref="WorkflowCore.Interface.IWorkflowRegistry"/>
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Version of workflow scheme
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Human-readable workflow description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Workflow steps
        /// </summary>
        public WorkflowStepCollection Steps { get; set; } = new WorkflowStepCollection();

        /// <summary>
        /// Type used as data for workflow
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// Default error handling policy for workflow
        /// </summary>
        public WorkflowErrorHandling DefaultErrorBehavior { get; set; }

        /// <summary>
        /// Default retry interval for workflow. Used if <see cref="DefaultErrorBehavior"/> or <see cref="WorkflowStep.ErrorBehavior"/> is set to <see cref="WorkflowErrorHandling.Retry"/>
        /// </summary>
        public TimeSpan? DefaultErrorRetryInterval { get; set; }
    }
}