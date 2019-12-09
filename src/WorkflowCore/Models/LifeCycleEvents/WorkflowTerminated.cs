using System;
using JetBrains.Annotations;

namespace WorkflowCore.Models.LifeCycleEvents
{
    /// <summary>
    /// Event for terminated workflow
    /// </summary>
    [PublicAPI]
    // ReSharper disable once InconsistentNaming
    public class WorkflowTerminated : LifeCycleEvent
    {
    }
}
