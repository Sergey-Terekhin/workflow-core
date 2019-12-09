using System;
using System.Linq;
using JetBrains.Annotations;

namespace WorkflowCore.Models
{
    /// <summary>
    /// Policy for error handling
    /// </summary>
    [PublicAPI]
    // todo think about replacing to Polly
    // ReSharper disable once InconsistentNaming
    public enum WorkflowErrorHandling 
    { 
        /// <summary>
        /// Retry operation
        /// </summary>
        Retry = 0, 
        /// <summary>
        /// Suspend workflow or step and wait for user reaction
        /// </summary>
        Suspend = 1, 
        /// <summary>
        /// Terminate workflow
        /// </summary>
        Terminate = 2,
        /// <summary>
        /// Execute compensating action
        /// </summary>
        Compensate = 3
    }
}