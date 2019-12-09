using System;
using JetBrains.Annotations;

namespace WorkflowCore.Models.LifeCycleEvents
{
    /// <summary>
    /// Event for started workflow
    /// </summary>
    [PublicAPI]
    // ReSharper disable once InconsistentNaming
    public class WorkflowStarted : LifeCycleEvent
    {
    }
}
