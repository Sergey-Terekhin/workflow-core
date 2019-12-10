using System;
using WorkflowCore.Interface;
using WorkflowCore.Models;

// ReSharper disable once CheckNamespace
namespace WorkflowCore.Services
{
    public class ReturnStepBuilder<TData, TStepBody, TParentStep> : IContainerStepBuilder<TData, TStepBody, TParentStep>
        where TStepBody : IStepBody
        where TParentStep : IStepBody
    {
        private readonly IStepBuilder<TData, TParentStep> _referenceBuilder;
        private readonly IWorkflowBuilder<TData> _builder;
        private readonly WorkflowStep<TStepBody> _step;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="workflowBuilder">Parent workflow builder</param>
        /// <param name="step">Workflow step</param>
        /// <param name="referenceBuilder"></param>
        public ReturnStepBuilder(IWorkflowBuilder<TData> workflowBuilder, WorkflowStep<TStepBody> step, IStepBuilder<TData, TParentStep> referenceBuilder)
        {
            _builder = workflowBuilder;
            _step = step;
            _referenceBuilder = referenceBuilder;
        }
        
        /// <inheritdoc />
        public IStepBuilder<TData, TParentStep> Do(Action<IWorkflowBuilder<TData>> builder)
        {
            builder.Invoke(_builder);
            _step.Children.Add(_step.Id + 1); //TODO: make more elegant

            return _referenceBuilder;
        }
    }
}
