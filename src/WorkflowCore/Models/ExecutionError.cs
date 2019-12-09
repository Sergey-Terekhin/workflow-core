using System;
// ReSharper disable InconsistentNaming

namespace WorkflowCore.Models
{
    /// <summary>
    /// Model for execution error
    /// </summary>
    public class ExecutionError
    {
        /// <summary>
        /// Error time
        /// </summary>
        public DateTime ErrorTime { get; set; }

        /// <summary>
        /// Identifier of workflow instance
        /// </summary>
        public string WorkflowId { get; set; }

        /// <summary>
        /// Identifier of execution pointer 
        /// </summary>
        public string ExecutionPointerId { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; }
    }
}
