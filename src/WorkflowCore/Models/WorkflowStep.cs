using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using WorkflowCore.Interface;
// ReSharper disable InconsistentNaming

namespace WorkflowCore.Models
{
    /// <summary>
    /// Base class for workflow steps
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public abstract class WorkflowStep
    {
        /// <summary>
        /// Type of step body. Used to construct it
        /// </summary>
        public abstract Type BodyType { get; }

        /// <summary>
        /// Identifier of the step
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        /// Name of the step
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// External identifier of the step
        /// </summary>
        public virtual string ExternalId { get; set; }

        /// <summary>
        /// List of identifiers of child steps
        /// </summary>
        public virtual List<int> Children { get; set; } = new List<int>();

        public virtual List<StepOutcome> Outcomes { get; set; } = new List<StepOutcome>();

        /// <summary>
        /// List of input parameters
        /// </summary>
        public virtual List<IStepParameter> Inputs { get; set; } = new List<IStepParameter>();

        /// <summary>
        /// List of output parameters
        /// </summary>
        public virtual List<IStepParameter> Outputs { get; set; } = new List<IStepParameter>();

        /// <summary>
        /// Error handling policy for step. If not set, policy for workflow will be used
        /// </summary>
        public virtual WorkflowErrorHandling? ErrorBehavior { get; set; }

        /// <summary>
        /// Interval between retries. Used if <see cref="ErrorBehavior"/> is <see cref="WorkflowErrorHandling.Retry"/>
        /// </summary>
        public virtual TimeSpan? RetryInterval { get; set; }

        /// <summary>
        /// Identifier of compensation step. Used if <see cref="ErrorBehavior"/> is <see cref="WorkflowErrorHandling.Compensate"/>
        /// </summary>
        public virtual int? CompensationStepId { get; set; }

        /// <summary>
        /// If set, steps will continue execution after compensation
        /// </summary>
        public virtual bool ResumeChildrenAfterCompensation => true;

        /// <summary>
        /// If set, steps will execute in reverse order for compensation
        /// </summary>
        public virtual bool RevertChildrenAfterCompensation => false;

        /// <summary>
        /// Expression for cancel condition
        /// </summary>
        public virtual LambdaExpression CancelCondition { get; set; }

        public bool ProceedOnCancel { get; set; } = false;

        public virtual ExecutionPipelineDirective InitForExecution(WorkflowExecutorResult executorResult, WorkflowDefinition definition, WorkflowInstance workflow, IExecutionPointer executionPointer)
        {
            return ExecutionPipelineDirective.Next;
        }

        public virtual ExecutionPipelineDirective BeforeExecute(WorkflowExecutorResult executorResult, IStepExecutionContext context, IExecutionPointer executionPointer, IStepBody body)
        {
            return ExecutionPipelineDirective.Next;
        }

        public virtual void AfterExecute(WorkflowExecutorResult executorResult, IStepExecutionContext context, ExecutionResult stepResult, IExecutionPointer executionPointer)
        {            
        }

        public virtual void PrimeForRetry(IExecutionPointer pointer)
        {
        }

        /// <summary>
        /// Called after every workflow execution round,
        /// every execution pointer with no end time, even if this step was not executed in this round
        /// </summary>
        /// <param name="executorResult"></param>
        /// <param name="defintion"></param>
        /// <param name="workflow"></param>
        /// <param name="executionPointer"></param>
        public virtual void AfterWorkflowIteration(WorkflowExecutorResult executorResult, WorkflowDefinition defintion, WorkflowInstance workflow, IExecutionPointer executionPointer)
        {
            
        }

        /// <summary>
        /// Create step body
        /// </summary>
        /// <param name="serviceProvider">Service provider to resolve dependencies</param>
        /// <returns>Instance of step body</returns>
        public virtual IStepBody ConstructBody(IServiceProvider serviceProvider)
        {
            var body = serviceProvider.GetService(BodyType) as IStepBody;
            if (body == null)
            {
                var stepCtor = BodyType.GetConstructor(new Type[] { });
                if (stepCtor != null)
                    body = stepCtor.Invoke(null) as IStepBody;
            }
            return body;
        }
    }

    /// <summary>
    /// Typed workflow step
    /// </summary>
    /// <typeparam name="TStepBody">Type of step body</typeparam>
    public class WorkflowStep<TStepBody> : WorkflowStep
        where TStepBody : IStepBody 
    {
        /// <inheritdoc />
        public override Type BodyType => typeof(TStepBody);
    }

	public enum ExecutionPipelineDirective 
    { 
        
        Next = 0, 
        /// <summary>
        /// Defer execution
        /// </summary>
        Defer = 1, 
        /// <summary>
        /// End workflow
        /// </summary>
        EndWorkflow = 2 
    }
}
