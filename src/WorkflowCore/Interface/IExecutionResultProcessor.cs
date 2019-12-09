using System;
using WorkflowCore.Models;

namespace WorkflowCore.Interface
{
    public interface IExecutionResultProcessor
    {
        /// <summary>
        /// Handles exception thrown while executing workflow step
        /// </summary>
        /// <param name="workflow">Workflow instance</param>
        /// <param name="def">Definition of the workflow (scheme) </param>
        /// <param name="pointer">Execution pointer instance</param>
        /// <param name="step">Workflow step threw exception </param>
        /// <param name="exception">Exception to handle</param>
        void HandleStepException(WorkflowInstance workflow, WorkflowDefinition def, IExecutionPointer pointer, WorkflowStep step, Exception exception);
        
        void ProcessExecutionResult(WorkflowInstance workflow, WorkflowDefinition def, IExecutionPointer pointer, WorkflowStep step, ExecutionResult result, WorkflowExecutorResult workflowResult);
    }
}