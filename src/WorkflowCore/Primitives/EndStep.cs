using System;
using WorkflowCore.Models;

namespace WorkflowCore.Primitives
{
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
