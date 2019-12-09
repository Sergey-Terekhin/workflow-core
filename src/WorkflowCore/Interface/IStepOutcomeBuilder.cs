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

        IStepBuilder<TData, TStep> Then<TStep>(Action<IStepBuilder<TData, TStep>> stepSetup = null) where TStep : IStepBody;

        IStepBuilder<TData, TStep> Then<TStep>(IStepBuilder<TData, TStep> step) where TStep : IStepBody;

        IStepBuilder<TData, InlineStepBody> Then(Func<IStepExecutionContext, ExecutionResult> body);

        void EndWorkflow();
    }
}