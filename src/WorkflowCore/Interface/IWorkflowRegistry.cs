using WorkflowCore.Models;

namespace WorkflowCore.Interface
{
    /// <summary>
    /// Interface for the workflows registry
    /// </summary>
    //todo make async
    public interface IWorkflowRegistry
    {
        /// <summary>
        /// Adds new workflow to the registry
        /// </summary>
        /// <param name="workflow">Workflow instance to register</param>
        void RegisterWorkflow(IWorkflow workflow);
        /// <summary>
        /// Adds new workflow definition to the registry
        /// </summary>
        /// <param name="definition">Workflow definition to register</param>
        void RegisterWorkflow(WorkflowDefinition definition);
        /// <summary>
        /// Adds new typed workflow to the registry
        /// </summary>
        /// <param name="workflow">Workflow instance to register</param>
        void RegisterWorkflow<TData>(IWorkflow<TData> workflow) where TData : new();
        /// <summary>
        /// Returns workflow definition by identifier and, optionally, version
        /// </summary>
        /// <param name="workflowId">Workflow identifier</param>
        /// <param name="version">Workflow version</param>
        /// <returns></returns>
        WorkflowDefinition GetDefinition(string workflowId, int? version = null);
    }
}
