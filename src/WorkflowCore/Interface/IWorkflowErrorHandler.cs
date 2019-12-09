using System;
using System.Collections.Generic;
using WorkflowCore.Models;

namespace WorkflowCore.Interface
{
    /// <summary>
    /// Interface to handle errors in the workflow
    /// </summary>
    public interface IWorkflowErrorHandler
    {
        /// <summary>
        /// Returns policy for error handling
        /// </summary>
        WorkflowErrorHandling Type { get; }
        /// <summary>
        /// Handle error
        /// </summary>
        /// <param name="workflow">Workflow instance</param>
        /// <param name="def">Workflow definition</param>
        /// <param name="pointer">Instance of the execution pointer caused the error</param>
        /// <param name="step">Instance of the step caused the error</param>
        /// <param name="exception">Error to handle</param>
        /// <param name="bubbleUpQueue"></param>
        void Handle(WorkflowInstance workflow, WorkflowDefinition def, IExecutionPointer pointer, WorkflowStep step, Exception exception, Queue<IExecutionPointer> bubbleUpQueue);
    }
}
