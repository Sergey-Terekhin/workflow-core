using System;
using System.Collections.Generic;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.Models.LifeCycleEvents;

namespace WorkflowCore.Services.ErrorHandlers
{
    /// <summary>
    /// Handles errors in steps with error handling policy set to <see cref="WorkflowErrorHandling.Suspend"/>
    /// </summary>
    public class SuspendHandler : IWorkflowErrorHandler
    {
        private readonly ILifeCycleEventPublisher _eventPublisher;
        private readonly IDateTimeProvider _datetimeProvider;

        /// <inheritdoc />
        public WorkflowErrorHandling Type => WorkflowErrorHandling.Suspend;

        /// <summary>
        /// ctor
        /// </summary>
        public SuspendHandler(ILifeCycleEventPublisher eventPublisher, IDateTimeProvider datetimeProvider)
        {
            _eventPublisher = eventPublisher;
            _datetimeProvider = datetimeProvider;
        }

        /// <inheritdoc />
        public void Handle(WorkflowInstance workflow, WorkflowDefinition def, IExecutionPointer pointer, WorkflowStep step, Exception exception, Queue<IExecutionPointer> bubbleUpQueue)
        {
            workflow.Status = WorkflowStatus.Suspended;
            _eventPublisher.PublishNotification(new WorkflowSuspended
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
