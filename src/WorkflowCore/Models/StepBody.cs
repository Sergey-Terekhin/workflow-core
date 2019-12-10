using System;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace WorkflowCore.Models
{
    /// <summary>
    /// Base implementation of step body which executes synchronously 
    /// </summary>
    public abstract class StepBody : IStepBody
    {
        /// <summary>
        /// Run step synchronously
        /// </summary>
        /// <param name="context">Instance of step context</param>
        /// <returns></returns>
        public abstract ExecutionResult Run(IStepExecutionContext context);

        /// <inheritdoc />
        public Task<ExecutionResult> RunAsync(IStepExecutionContext context)
        {
            return Task.FromResult(Run(context));
        }
    }
}