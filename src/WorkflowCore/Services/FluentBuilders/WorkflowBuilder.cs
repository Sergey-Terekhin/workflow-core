using System;
using System.Collections.Generic;
using System.Linq;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.Primitives;
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming

namespace WorkflowCore.Services
{
    /// <summary>
    /// Implementation of <see cref="IWorkflowBuilder"/>
    /// </summary>
    public class WorkflowBuilder : IWorkflowBuilder
    {
        /// <summary>
        /// List of workflow steps to execute
        /// </summary>
        protected List<WorkflowStep> Steps { get; set; } = new List<WorkflowStep>();

        /// <summary>
        /// Default value of error handling policy. It's applied if policy is not specified for concrete workflow step
        /// </summary>
        protected WorkflowErrorHandling DefaultErrorBehavior = WorkflowErrorHandling.Retry;

        
        /// <summary>
        /// Default value of retry interval. It's applied if effective error handling policy is set to <see cref="WorkflowErrorHandling.Retry"/>
        /// and Retry Interval is not specified for concrete workflow step
        /// </summary>
        protected TimeSpan? DefaultErrorRetryInterval;

        /// <inheritdoc />
        public int LastStep => Steps.Max(x => x.Id);

        /// <inheritdoc />
        public IWorkflowBuilder<T> UseData<T>()
        {
            IWorkflowBuilder<T> result = new WorkflowBuilder<T>(Steps);
            return result;
        }

        /// <inheritdoc />
        public virtual WorkflowDefinition Build(string id, int version)
        {
            AttachExternalIds();
            return new WorkflowDefinition
            {
                Id = id,
                Version = version,
                Steps = new WorkflowStepCollection(Steps),
                DefaultErrorBehavior = DefaultErrorBehavior,
                DefaultErrorRetryInterval = DefaultErrorRetryInterval
            };
        }

        /// <inheritdoc />
        public void AddStep(WorkflowStep step)
        {
            step.Id = Steps.Count();
            Steps.Add(step);
        }

        private void AttachExternalIds()
        {
            foreach (var step in Steps)
            {
                foreach (var outcome in step.Outcomes.Where(x => !string.IsNullOrEmpty(x.ExternalNextStepId)))
                {
                    if (Steps.All(x => x.ExternalId != outcome.ExternalNextStepId))
                        throw new KeyNotFoundException($"Cannot find step id {outcome.ExternalNextStepId}");

                    outcome.NextStep = Steps.Single(x => x.ExternalId == outcome.ExternalNextStepId).Id;
                }
            }
        }

    }

    /// <summary>
    /// Implementation of <see cref="IWorkflowBuilder{TData}"/>
    /// </summary>
    /// <typeparam name="TData">Type of workflow data</typeparam>
    public class WorkflowBuilder<TData> : WorkflowBuilder, IWorkflowBuilder<TData>
    {
        /// <inheritdoc />
        public override WorkflowDefinition Build(string id, int version)
        {
            var result = base.Build(id, version);
            result.DataType = typeof(TData);
            return result;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="steps">workflow steps</param>
        public WorkflowBuilder(IEnumerable<WorkflowStep> steps)
        {
            Steps.AddRange(steps);
        }

        /// <inheritdoc />
        public IStepBuilder<TData, TStep> StartWith<TStep>(Action<IStepBuilder<TData, TStep>> stepSetup = null)
            where TStep : IStepBody
        {
            WorkflowStep<TStep> step = new WorkflowStep<TStep>();
            var stepBuilder = new StepBuilder<TData, TStep>(this, step);

            stepSetup?.Invoke(stepBuilder);

            step.Name ??= typeof(TStep).Name;
            AddStep(step);
            return stepBuilder;
        }

        /// <inheritdoc />
        public IStepBuilder<TData, InlineStepBody> StartWith(Func<IStepExecutionContext, ExecutionResult> body)
        {
            WorkflowStepInline newStep = new WorkflowStepInline();
            newStep.Body = body;
            var stepBuilder = new StepBuilder<TData, InlineStepBody>(this, newStep);
            AddStep(newStep);
            return stepBuilder;
        }

        /// <inheritdoc />
        public IStepBuilder<TData, ActionStepBody> StartWith(Action<IStepExecutionContext> body)
        {
            var newStep = new WorkflowStep<ActionStepBody>();
            AddStep(newStep);
            var stepBuilder = new StepBuilder<TData, ActionStepBody>(this, newStep);
            stepBuilder.Input(x => x.Body, x => body);
            return stepBuilder;
        }

        /// <inheritdoc />
        public IReadOnlyList<WorkflowStep> GetUpstreamSteps(int id)
        {
            return Steps.Where(x => x.Outcomes.Any(y => y.NextStep == id)).ToList();
        }
    }
        
}
