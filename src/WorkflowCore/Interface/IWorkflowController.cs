using System;
using System.Threading.Tasks;

namespace WorkflowCore.Interface
{
    /// <summary>
    /// Interface to control workflow 
    /// </summary>
    public interface IWorkflowController
    {
        /// <summary>
        /// Start workflow with specified id and data
        /// </summary>
        /// <param name="workflowId">Identifier of workflow definition (scheme)</param>
        /// <param name="data">Data which will be attached to the workflow </param>
        /// <param name="reference"></param>
        /// <returns>Identifier of started workflow instance</returns>
        Task<string> StartWorkflow(string workflowId, object data = null, string reference = null);

        /// <summary>
        /// Start workflow with specified id and data
        /// </summary>
        /// <param name="workflowId">Identifier of workflow definition (scheme)</param>
        /// <param name="version">Version of workflow definition</param>
        /// <param name="data">Data which will be attached to the workflow </param>
        /// <param name="reference"></param>
        /// <returns>Identifier of started workflow instance</returns>
        Task<string> StartWorkflow(string workflowId, int? version, object data = null, string reference = null);

        /// <summary>
        /// Start workflow with specified id and data
        /// </summary>
        /// <param name="workflowId">Identifier of workflow definition (scheme)</param>
        /// <param name="data">Data which will be attached to the workflow </param>
        /// <param name="reference"></param>
        /// <returns>Identifier of started workflow instance</returns>
        Task<string> StartWorkflow<TData>(string workflowId, TData data = null, string reference = null)
            where TData : class, new();

        /// <summary>
        /// Start workflow with specified id and data
        /// </summary>
        /// <param name="workflowId">Identifier of workflow definition (scheme)</param>
        /// <param name="version">Version of workflow definition</param>
        /// <param name="data">Data which will be attached to the workflow </param>
        /// <param name="reference"></param>
        /// <returns>Identifier of started workflow instance</returns>
        Task<string> StartWorkflow<TData>(string workflowId, int? version, TData data = null, string reference = null)
            where TData : class, new();

        /// <summary>
        /// Publish event
        /// </summary>
        /// <param name="eventName">Event name</param>
        /// <param name="eventKey">Event key</param>
        /// <param name="eventData">Event data</param>
        /// <param name="effectiveDate">Event time</param>
        /// <returns></returns>
        Task PublishEvent(string eventName, string eventKey, object eventData, DateTime? effectiveDate = null);

        /// <summary>
        /// Register workflow
        /// </summary>
        /// <typeparam name="TWorkflow">Workflow type</typeparam>
        void RegisterWorkflow<TWorkflow>() where TWorkflow : IWorkflow, new();
        
        /// <summary>
        /// Register workflow
        /// </summary>
        /// <typeparam name="TWorkflow">Workflow type</typeparam>
        /// <typeparam name="TData">Workflow data type</typeparam>
        void RegisterWorkflow<TWorkflow, TData>() where TWorkflow : IWorkflow<TData>, new() where TData : new();

        /// <summary>
        /// Suspend the execution of a given workflow until <see cref="ResumeWorkflow"/> is called
        /// </summary>
        /// <param name="workflowId"></param>
        /// <returns></returns>
        Task<bool> SuspendWorkflow(string workflowId);

        /// <summary>
        /// Resume a previously suspended workflow
        /// </summary>
        /// <param name="workflowId"></param>
        /// <returns></returns>
        Task<bool> ResumeWorkflow(string workflowId);

        /// <summary>
        /// Permanently terminate the exeuction of a given workflow
        /// </summary>
        /// <param name="workflowId"></param>
        /// <returns></returns>
        Task<bool> TerminateWorkflow(string workflowId);
    }
}