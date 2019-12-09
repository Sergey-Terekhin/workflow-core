using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace WorkflowCore.Models.DefinitionStorage.v1
{
    /// <inheritdoc />
    [PublicAPI]
    public class DefinitionSourceV1 : DefinitionSource
    {
        /// <summary>
        /// Workflow data type name. If not set, <see cref="object"/> will be used
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Default error handling policy for workflow
        /// </summary>
        public WorkflowErrorHandling DefaultErrorBehavior { get; set; }

        /// <summary>
        /// Default retry interval for workflow. Used if <see cref="DefaultErrorBehavior"/> or <see cref="StepSourceV1.ErrorBehavior"/> is set to <see cref="WorkflowErrorHandling.Retry"/>
        /// </summary>
        public TimeSpan? DefaultErrorRetryInterval { get; set; }

        /// <summary>
        /// List of workflow steps
        /// </summary>
        public List<StepSourceV1> Steps { get; set; } = new List<StepSourceV1>();
    }
}
