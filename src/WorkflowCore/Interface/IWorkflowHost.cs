using System;
using WorkflowCore.Models;
using WorkflowCore.Models.LifeCycleEvents;

namespace WorkflowCore.Interface
{
    /// <summary>
    /// Interface for workflow host which is responsible for registering workflows as for starting and stopping them 
    /// </summary>
    public interface IWorkflowHost : IWorkflowController, IBackgroundTask
    {
        /// <summary>
        /// Event fired then error in the step is occured
        /// </summary>
        event StepErrorEventHandler OnStepError;
        
        /// <summary>
        /// Event fired then workflow moves from state to state
        /// </summary>
        event LifeCycleEventHandler OnLifeCycleEvent;
        
        /// <summary>
        /// Report workflow error
        /// </summary>
        /// <param name="workflow">Instance of workflow</param>
        /// <param name="step">Instance of workflow step</param>
        /// <param name="exception">Error</param>
        void ReportStepError(WorkflowInstance workflow, WorkflowStep step, Exception exception);
    }

    /// <summary>
    /// Delegate for <see cref="IWorkflowHost.OnStepError"/> event
    /// </summary>
    /// <param name="workflow">Instance of workflow</param>
    /// <param name="step">Instance of workflow step</param>
    /// <param name="exception">Error</param>
    public delegate void StepErrorEventHandler(WorkflowInstance workflow, WorkflowStep step, Exception exception);
    
    /// <summary>
    /// Delegate for <see cref="IWorkflowHost.OnLifeCycleEvent"/> event
    /// </summary>
    /// <param name="evt">event instance</param>
    public delegate void LifeCycleEventHandler(LifeCycleEvent evt);
}