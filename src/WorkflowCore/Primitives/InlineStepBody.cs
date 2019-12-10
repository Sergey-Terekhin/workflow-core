using System;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Primitives
{
    /// <summary>
    /// Step body with function to execute
    /// </summary>
    public class InlineStepBody : StepBody
    {

        private readonly Func<IStepExecutionContext, ExecutionResult> _body;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="body">Function to execute</param>
        public InlineStepBody(Func<IStepExecutionContext, ExecutionResult> body)
        {
            _body = body;
        }

        /// <inheritdoc />
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            return _body.Invoke(context);
        }
    }
}
