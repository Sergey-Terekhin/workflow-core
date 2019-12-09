using System;
using JetBrains.Annotations;

namespace WorkflowCore.Models.LifeCycleEvents
{
    /// <summary>
    /// Event for resumed workflow
    /// </summary>
    [PublicAPI]
    // ReSharper disable once InconsistentNaming
    public class WorkflowResumed : LifeCycleEvent
    {
    }
}
