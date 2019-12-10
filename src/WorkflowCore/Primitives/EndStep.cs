using System;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Primitives
{
    /// <summary>
    /// Last step in workflow 
    /// </summary>
    public class EndStep : WorkflowStep
    {
        /// <inheritdoc />
        public override Type BodyType => null;

        /// <inheritdoc />
        public override ExecutionPipelineDirective InitForExecution(
            WorkflowExecutorResult executorResult, 
            WorkflowDefinition definition, 
            WorkflowInstance workflow, 
            IExecutionPointer executionPointer)
        {
            return ExecutionPipelineDirective.EndWorkflow;
        }
    }
}
