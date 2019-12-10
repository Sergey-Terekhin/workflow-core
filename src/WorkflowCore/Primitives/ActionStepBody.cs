using System;
using JetBrains.Annotations;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Primitives
{
    /// <summary>
    /// Step body which execute action specified in the <see cref="Body"/> property
    /// </summary>
    [PublicAPI]
    public class ActionStepBody : StepBody
    {
        /// <summary>
        /// action to execute
        /// </summary>
        public Action<IStepExecutionContext> Body { get; set; }

        /// <inheritdoc />
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Body?.Invoke(context);
            return ExecutionResult.Next();
        }
    }
}