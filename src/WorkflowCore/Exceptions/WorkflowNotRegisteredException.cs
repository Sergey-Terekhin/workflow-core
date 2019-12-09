using System;

namespace WorkflowCore.Exceptions
{
    /// <summary>
    /// Exception thrown if workflow with specified identifier and version is not registered
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class WorkflowNotRegisteredException : WorkflowBaseException
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="workflowId">Identifier of workflow</param>
        /// <param name="version">Version of workflow</param>
        public WorkflowNotRegisteredException(string workflowId, int? version)
            : base($"Workflow {workflowId} {version} is not registered")
        {
        }
    }
}
