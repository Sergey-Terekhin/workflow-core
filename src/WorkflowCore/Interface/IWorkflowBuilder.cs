using System;
using System.Collections.Generic;
using WorkflowCore.Models;
using WorkflowCore.Primitives;

namespace WorkflowCore.Interface
{
    /// <summary>
    /// Interface to  build workflow definition
    /// </summary>
    public interface IWorkflowBuilder
    {
        /// <summary>
        /// identifier of the last step
        /// </summary>
        int LastStep { get; }

        /// <summary>
        /// Returns typed instance of <see cref="IWorkflowBuilder{TData}"/>
        /// </summary>
        /// <typeparam name="T">Data type to use</typeparam>
        /// <returns></returns>
        IWorkflowBuilder<T> UseData<T>();

        /// <summary>
        /// Create workflow definition with specific id and version
        /// </summary>
        /// <param name="id">Workflow id</param>
        /// <param name="version">Workflow version</param>
        /// <returns></returns>
        WorkflowDefinition Build(string id, int version);

        /// <summary>
        /// Adds new step to workflow
        /// </summary>
        /// <param name="step">Step to add</param>
        void AddStep(WorkflowStep step);
    }

    /// <summary>
    /// Typed builder
    /// </summary>
    /// <typeparam name="TData">Workflow data type</typeparam>
    public interface IWorkflowBuilder<TData> : IWorkflowBuilder
    {
        /// <summary>
        /// Specifies first step for the workflow
        /// </summary>
        /// <param name="stepSetup">Action to setup step</param>
        /// <typeparam name="TStep">Step's type</typeparam>
        IStepBuilder<TData, TStep> StartWith<TStep>(Action<IStepBuilder<TData, TStep>> stepSetup = null)
            where TStep : IStepBody;

        /// <summary>
        /// Specifies first step for the workflow as inline function
        /// </summary>
        /// <param name="body">Function to execute as workflow step</param>
        IStepBuilder<TData, InlineStepBody> StartWith(Func<IStepExecutionContext, ExecutionResult> body);

        /// <summary>
        /// Specifies first step for the workflow as action
        /// </summary>
        /// <param name="body">Action to execute as workflow step</param>
        IStepBuilder<TData, ActionStepBody> StartWith(Action<IStepExecutionContext> body);

        /// <summary>
        /// Return all parent steps for step with specified identifier
        /// </summary>
        /// <param name="id">Identifier of the step</param>
        /// <returns></returns>
        IReadOnlyList<WorkflowStep> GetUpstreamSteps(int id);
    }
}