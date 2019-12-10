using System;
using System.Collections.Generic;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.Models.LifeCycleEvents;

namespace WorkflowCore.Services.ErrorHandlers
{
    /// <summary>
    /// Handles errors in steps with error handling policy set to <see cref="WorkflowErrorHandling.Terminate"/>
    /// </summary>
    public class TerminateHandler : IWorkflowErrorHandler
    {
        private readonly ILifeCycleEventPublisher _eventPublisher;
        private readonly IDateTimeProvider _datetimeProvider;

        /// <inheritdoc />
        public WorkflowErrorHandling Type => WorkflowErrorHandling.Terminate;

        /// <summary>
        /// ctor
        /// </summary>
        public TerminateHandler(ILifeCycleEventPublisher eventPublisher, IDateTimeProvider datetimeProvider)
        {
            _eventPublisher = eventPublisher;
            _datetimeProvider = datetimeProvider;
        }

        /// <inheritdoc />
        public void Handle(WorkflowInstance workflow, WorkflowDefinition def, IExecutionPointer pointer, WorkflowStep step, Exception exception, Queue<IExecutionPointer> bubbleUpQueue)
        {
            workflow.Status = WorkflowStatus.Terminated;
            _eventPublisher.PublishNotification(new WorkflowTerminated
            {
                EventTimeUtc = _datetimeProvider.Now,
                Reference = workflow.Reference,
                WorkflowInstanceId = workflow.Id,
                WorkflowDefinitionId = workflow.WorkflowDefinitionId,
                Version = workflow.Version
            });
        }
    }
}
