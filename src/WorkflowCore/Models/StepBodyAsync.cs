using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace WorkflowCore.Models
{
    /// <summary>
    /// Step body which executes asynchronously
    /// </summary>
    public abstract class StepBodyAsync : IStepBody
    {
        /// <inheritdoc />
        public abstract Task<ExecutionResult> RunAsync(IStepExecutionContext context);
    }
}
