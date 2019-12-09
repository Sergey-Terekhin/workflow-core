using System;
using System.Collections.Generic;

namespace WorkflowCore.Models
{
    public interface IExecutionPointer
    {
        string Id { get; set; }
        int StepId { get; set; }
        bool Active { get; set; }
        DateTime? SleepUntil { get; set; }
        object PersistenceData { get; set; }
        DateTime? StartTime { get; set; }
        DateTime? EndTime { get; set; }
        string EventName { get; set; }
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
        PointerStatus Status { get; set; }
        IReadOnlyCollection<string> Scope { get; set; }
    }
}