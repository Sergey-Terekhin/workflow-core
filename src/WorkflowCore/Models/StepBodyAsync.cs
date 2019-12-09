using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace WorkflowCore.Models
{
    public abstract class StepBodyAsync : IStepBody
    {
        /// <inheritdoc />
        public abstract Task<ExecutionResult> RunAsync(IStepExecutionContext context);
    }
}
