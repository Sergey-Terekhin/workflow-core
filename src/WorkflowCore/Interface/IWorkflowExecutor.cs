using System.Threading.Tasks;
using WorkflowCore.Models;

namespace WorkflowCore.Interface
{
    /// <summary>
    /// Interface for workflow executor
    /// </summary>
    public interface IWorkflowExecutor
    {
        /// <summary>
        /// Execute specified workflow
        /// </summary>
        /// <param name="workflow">Workflow instance</param>
        /// <returns></returns>
        Task<WorkflowExecutorResult> Execute(WorkflowInstance workflow);
    }
}