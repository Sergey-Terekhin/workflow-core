using System;
using System.Collections.Generic;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Services.ErrorHandlers
{
    public class RetryHandler : IWorkflowErrorHandler
    {
        private readonly IDateTimeProvider _datetimeProvider;
        private readonly WorkflowOptions _options;
        public WorkflowErrorHandling Type => WorkflowErrorHandling.Retry;

        public RetryHandler(IDateTimeProvider datetimeProvider, WorkflowOptions options)
        {
            _datetimeProvider = datetimeProvider;
            _options = options;
        }

        /// <inheritdoc />
        public void Handle(WorkflowInstance workflow, WorkflowDefinition def, IExecutionPointer pointer, WorkflowStep step, Exception exception, Queue<IExecutionPointer> bubbleUpQueue)
        {
            pointer.RetryCount++;
            pointer.SleepUntil = _datetimeProvider.Now.ToUniversalTime().Add(step.RetryInterval ?? def.DefaultErrorRetryInterval ?? _options.ErrorRetryInterval);
            step.PrimeForRetry(pointer);
        }
    }
}
