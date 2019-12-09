using System;
using WorkflowCore.Models;

namespace WorkflowCore.Interface
{
    /// <summary>
    /// Service to process <see cref="WorkflowStep.CancelCondition"/>
    /// </summary>
    public interface ICancellationProcessor
    {
        /// <summary>
        /// Process cancellations for workflow
        /// </summary>
        /// <param name="workflow">Workflow instance to process</param>
        /// <param name="workflowDef">Workflow scheme</param>
        /// <param name="executionResult"></param>
        void ProcessCancellations(WorkflowInstance workflow, WorkflowDefinition workflowDef, WorkflowExecutorResult executionResult);
    }
}
