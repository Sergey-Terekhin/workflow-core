using System;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.Primitives;
// ReSharper disable CheckNamespace

namespace WorkflowCore.Services
{
    public class ParallelStepBuilder<TData, TStepBody> : IParallelStepBuilder<TData, TStepBody>
        where TStepBody : IStepBody
    {
        private readonly IStepBuilder<TData, Sequence> _referenceBuilder;
        private readonly IWorkflowBuilder<TData> _builder;
        private readonly WorkflowStep<TStepBody> _step;
        
        /// <summary>
        /// ctor
        /// </summary>
        public ParallelStepBuilder(IWorkflowBuilder<TData> workflowBuilder, IStepBuilder<TData, TStepBody> stepBuilder, IStepBuilder<TData, Sequence> referenceBuilder)
        {
            _builder = workflowBuilder;
            _step = stepBuilder.Step;
            _referenceBuilder = referenceBuilder;
        }
        
        /// <inheritdoc />
        public IParallelStepBuilder<TData, TStepBody> Do(Action<IWorkflowBuilder<TData>> builder)
        {
            var lastStep = _builder.LastStep;
            builder.Invoke(_builder);
            
            if (lastStep == _builder.LastStep)
                throw new NotSupportedException("Empty Do block not supported");
            
            _step.Children.Add(lastStep + 1); //TODO: make more elegant

            return this;
        }

        /// <inheritdoc />
        public IStepBuilder<TData, Sequence> Join()
        {
            return _referenceBuilder;
        }
    }
}
