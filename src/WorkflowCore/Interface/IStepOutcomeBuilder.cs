using System;
using JetBrains.Annotations;
using WorkflowCore.Models;
using WorkflowCore.Primitives;
// ReSharper disable InconsistentNaming

namespace WorkflowCore.Interface
{   
    /// <summary>
    /// Interface for step outcome builder 
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    [PublicAPI]
    public interface IStepOutcomeBuilder<TData>
    {
        /// <summary>
        /// Gets instance of <see cref="IWorkflowBuilder{TData}"/>
        /// </summary>
        IWorkflowBuilder<TData> WorkflowBuilder { get; }

        /// <summary>
        /// returns created instance of <see cref="StepOutcome"/>
        /// </summary>
        StepOutcome Outcome { get; }

        /// <summary>
        /// Specifies next action in the workflow
        /// </summary>
        /// <param name="stepSetup">Action to setup step</param>
        /// <typeparam name="TStep">Type of step body</typeparam>
        /// <returns></returns>
        IStepBuilder<TData, TStep> Then<TStep>(Action<IStepBuilder<TData, TStep>> stepSetup = null) where TStep : IStepBody;

        
        /// <summary>
        /// Specifies next action in the workflow
        /// </summary>
        /// <param name="step">Step builder</param>
        /// <typeparam name="TStep">Type of step body</typeparam>
        /// <returns></returns>
        IStepBuilder<TData, TStep> Then<TStep>(IStepBuilder<TData, TStep> step) where TStep : IStepBody;

        
        /// <summary>
        /// Specifies next action in the workflow
        /// </summary>
        /// <param name="body">Action to execute as workflow step</param>
        /// <returns></returns>
        IStepBuilder<TData, InlineStepBody> Then(Func<IStepExecutionContext, ExecutionResult> body);

        /// <summary>
        /// End workflow
        /// </summary>
        void EndWorkflow();
    }
}