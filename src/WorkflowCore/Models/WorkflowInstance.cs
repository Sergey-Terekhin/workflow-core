using System;
using System.Linq;
// ReSharper disable InconsistentNaming

namespace WorkflowCore.Models
{
    /// <summary>
    /// Model for workflow instance
    /// </summary>
    public class WorkflowInstance
    {
        /// <summary>
        /// Instance identifier
        /// </summary>
        public string Id { get; set; }
                
        /// <summary>
        /// Identifier of workflow definition (scheme)
        /// </summary>
        public string WorkflowDefinitionId { get; set; }

        /// <summary>
        /// Version of workflow
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Human-readable description of workflow
        /// </summary>
        public string Description { get; set; }

        public string Reference { get; set; }

        /// <summary>
        /// Collection of execution pointers
        /// </summary>
        public ExecutionPointerCollection ExecutionPointers { get; set; } = new ExecutionPointerCollection();

        /// <summary>
        /// Next execution time in ticks
        /// </summary>
        public long? NextExecution { get; set; }

        /// <summary>
        /// Current status of the instance
        /// </summary>
        public WorkflowStatus Status { get; set; }

        /// <summary>
        /// Data object attached to the instance
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Create timestamp of the instance
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Complete timestamp of the instance
        /// </summary>
        public DateTime? CompleteTime { get; set; }

        /// <summary>
        /// Checks if workflow branch is completed
        /// </summary>
        /// <param name="parentId">Identifier of parent execution pointer</param>
        /// <returns></returns>
        public bool IsBranchComplete(string parentId)
        {
            return ExecutionPointers
                .FindByScope(parentId)
                .All(x => x.EndTime != null);
        }
    }

    /// <summary>
    /// Workflow instance statuses
    /// </summary>
    public enum WorkflowStatus 
    { 
        /// <summary>
        /// Instance is waiting to be executed 
        /// </summary>
        Runnable = 0, 
        /// <summary>
        /// Instance is suspended and waiting for external event or data to be continued
        /// </summary>
        Suspended = 1, 
        /// <summary>
        /// Instance is complete
        /// </summary>
        Complete = 2, 
        /// <summary>
        /// Instance is terminated
        /// </summary>
        Terminated = 3 
    }
}
