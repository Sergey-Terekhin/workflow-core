using System;
using System.Collections.Generic;
using System.Linq;
using WorkflowCore.Models;

namespace WorkflowCore.Interface
{
    /// <summary>
    /// Interface for execution pointer
    /// </summary>
    /// <remarks>Execution pointer is ... //todo add description
    /// </remarks>
    public interface IExecutionPointer
    {
        /// <summary>
        /// Identifier of the execution pointer
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// Identifier of the step
        /// </summary>
        int StepId { get; set; }
        /// <summary>
        /// Active flag. If not set, pointer won't be executed
        /// </summary>
        bool Active { get; set; }
        DateTime? SleepUntil { get; set; }
        object PersistenceData { get; set; }
        DateTime? StartTime { get; set; }
        DateTime? EndTime { get; set; }
        /// <summary>
        /// Event name to wait
        /// </summary>
        string EventName { get; set; }
        /// <summary>
        /// Event key to wait
        /// </summary>
        string EventKey { get; set; }
        bool EventPublished { get; set; }
        object EventData { get; set; }
        Dictionary<string, object> ExtensionAttributes { get; }
        string StepName { get; set; }
        int RetryCount { get; set; }
        List<string> Children { get; set; }
        object ContextItem { get; set; }
        string PredecessorId { get; set; }
        object Outcome { get; set; }
        /// <summary>
        /// Status of the pointer
        /// </summary>
        PointerStatus Status { get; set; }
        IReadOnlyCollection<string> Scope { get; set; }
    }
}