using System;
using JetBrains.Annotations;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Primitives
{
    /// <summary>
    /// Step body for delay step
    /// </summary>
    [PublicAPI]
    public class Delay : StepBody
    {
        /// <summary>
        /// Time period to sleep
        /// </summary>
        public TimeSpan Period { get; set; }

        /// <inheritdoc />
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            if (context.PersistenceData != null)
            {
                return ExecutionResult.Next();
            }
            
            return ExecutionResult.Sleep(Period, true);
        }
    }
}
