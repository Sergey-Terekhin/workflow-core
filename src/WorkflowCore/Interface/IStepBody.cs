using System.Threading.Tasks;
using WorkflowCore.Models;

namespace WorkflowCore.Interface
{
    /// <summary>
    /// Base interface for step body. Contains main logic for workflow step
    /// </summary>
    public interface IStepBody
    {        
        /// <summary>
        /// Execute step asynchronously
        /// </summary>
        /// <param name="context">Instance of step context</param>
        /// <returns></returns>
        Task<ExecutionResult> RunAsync(IStepExecutionContext context);        
    }
}
