using System;
using System.Collections.Generic;
using System.Linq;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Services.ErrorHandlers
{
    /// <summary>
    /// Handles errors in steps with error handling policy set to <see cref="WorkflowErrorHandling.Compensate"/>
    /// </summary>
    public class CompensateHandler : IWorkflowErrorHandler
    {
        private readonly IExecutionPointerFactory _pointerFactory;
        private readonly IDateTimeProvider _datetimeProvider;

        /// <inheritdoc />
        public WorkflowErrorHandling Type => WorkflowErrorHandling.Compensate;

        /// <summary>
        /// ctor
        /// </summary>
        public CompensateHandler(IExecutionPointerFactory pointerFactory, IDateTimeProvider datetimeProvider)
        {
            _pointerFactory = pointerFactory;
            _datetimeProvider = datetimeProvider;
        }

        /// <inheritdoc />
        public void Handle(WorkflowInstance workflow, WorkflowDefinition def, IExecutionPointer exceptionPointer, WorkflowStep exceptionStep, Exception exception, Queue<IExecutionPointer> bubbleUpQueue)
        {
            var scope = new Stack<string>(exceptionPointer.Scope.Reverse());
            scope.Push(exceptionPointer.Id);
            
            while (scope.Any())
            {
                var pointerId = scope.Pop();
                var scopePointer = workflow.ExecutionPointers.FindById(pointerId);
                var scopeStep = def.Steps.FindById(scopePointer.StepId);

                var resume = true;
                var revert = false;
                
                var txnStack = new Stack<string>(scope.Reverse());
                while (txnStack.Count > 0)
                {
                    var parentId = txnStack.Pop();
                    var parentPointer = workflow.ExecutionPointers.FindById(parentId);
                    var parentStep = def.Steps.FindById(parentPointer.StepId);
                    if (!parentStep.ResumeChildrenAfterCompensation || parentStep.RevertChildrenAfterCompensation)
                    {
                        resume = parentStep.ResumeChildrenAfterCompensation;
                        revert = parentStep.RevertChildrenAfterCompensation;
                        break;
                    }
                }

                if ((scopeStep.ErrorBehavior ?? WorkflowErrorHandling.Compensate) != WorkflowErrorHandling.Compensate)
                {
                    bubbleUpQueue.Enqueue(scopePointer);
                    continue;
                }

                scopePointer.Active = false;
                scopePointer.EndTime = _datetimeProvider.Now.ToUniversalTime();
                scopePointer.Status = PointerStatus.Failed;

                if (scopeStep.CompensationStepId.HasValue)
                {
                    scopePointer.Status = PointerStatus.Compensated;

                    var compensationPointer = _pointerFactory.BuildCompensationPointer(def, scopePointer, exceptionPointer, scopeStep.CompensationStepId.Value);
                    workflow.ExecutionPointers.Add(compensationPointer);

                    if (resume)
                    {
                        foreach (var outcomeTarget in scopeStep.Outcomes.Where(x => x.GetValue(workflow.Data) == null))
                            workflow.ExecutionPointers.Add(_pointerFactory.BuildNextPointer(def, scopePointer, outcomeTarget));
                    }
                }

                if (revert)
                {
                    var prevSiblings = workflow.ExecutionPointers
                        .Where(x => scopePointer.Scope.SequenceEqual(x.Scope) && x.Id != scopePointer.Id && x.Status == PointerStatus.Complete)
                        .OrderByDescending(x => x.EndTime)
                        .ToList();

                    foreach (var siblingPointer in prevSiblings)
                    {
                        var siblingStep = def.Steps.FindById(siblingPointer.StepId);
                        if (siblingStep.CompensationStepId.HasValue)
                        {
                            var compensationPointer = _pointerFactory.BuildCompensationPointer(def, siblingPointer, exceptionPointer, siblingStep.CompensationStepId.Value);
                            workflow.ExecutionPointers.Add(compensationPointer);
                            siblingPointer.Status = PointerStatus.Compensated;
                        }
                    }
                }
            }
        }
    }
}
