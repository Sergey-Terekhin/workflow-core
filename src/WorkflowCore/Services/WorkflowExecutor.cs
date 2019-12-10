using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.Models.LifeCycleEvents;
// ReSharper disable InconsistentNaming

namespace WorkflowCore.Services
{
    /// <summary>
    /// Implementation of <see cref="IWorkflowExecutor"/>
    /// </summary>
    public sealed class WorkflowExecutor : IWorkflowExecutor
    {
        private readonly IWorkflowRegistry _registry;
        private readonly IScopeProvider _scopeProvider;
        private readonly IDateTimeProvider _datetimeProvider;
        private readonly ILogger _logger;
        private readonly IExecutionResultProcessor _executionResultProcessor;
        private readonly ICancellationProcessor _cancellationProcessor;
        private readonly ILifeCycleEventPublisher _publisher;
        private readonly WorkflowOptions _options;

        private readonly IWorkflowHost _host;

        /// <summary>
        /// ctor
        /// </summary>
        public WorkflowExecutor(
            IWorkflowHost host, 
            IWorkflowRegistry registry,
            IScopeProvider scopeProvider, 
            IDateTimeProvider datetimeProvider, 
            IExecutionResultProcessor executionResultProcessor,
            ILifeCycleEventPublisher publisher, 
            ICancellationProcessor cancellationProcessor,
            WorkflowOptions options, 
            ILoggerFactory loggerFactory)
        {
            _host = host;
            _scopeProvider = scopeProvider;
            _registry = registry;
            _datetimeProvider = datetimeProvider;
            _publisher = publisher;
            _cancellationProcessor = cancellationProcessor;
            _options = options;
            _logger = loggerFactory.CreateLogger<WorkflowExecutor>();
            _executionResultProcessor = executionResultProcessor;
        }

        /// <inheritdoc />
        public async Task<WorkflowExecutorResult> Execute(WorkflowInstance workflow)
        {
            var wfResult = new WorkflowExecutorResult();

            var exePointers = new List<IExecutionPointer>(workflow.ExecutionPointers.Where(x => x.Active && (!x.SleepUntil.HasValue || x.SleepUntil < _datetimeProvider.Now.ToUniversalTime())));
            var def = _registry.GetDefinition(workflow.WorkflowDefinitionId, workflow.Version);
            if (def == null)
            {
                _logger.LogError(
                    WellKnownLoggingEventIds.WorkflowNotExist,
                    "Workflow {WorkflowDefinitionId} version {Version} is not registered",
                    workflow.WorkflowDefinitionId,
                    workflow.Version);
                return wfResult;
            }

            _cancellationProcessor.ProcessCancellations(workflow, def, wfResult);

            foreach (var pointer in exePointers)
            {
                if (!pointer.Active)
                    continue;

                var step = def.Steps.FindById(pointer.StepId);
                if (step == null)
                {
                    _logger.LogError(
                        WellKnownLoggingEventIds.WorkflowStepNotExist,
                        "Unable to find step {StepId} in workflow definition",
                        pointer.StepId);

                    pointer.SleepUntil = _datetimeProvider.Now.ToUniversalTime().Add(_options.ErrorRetryInterval);
                    wfResult.Errors.Add(new ExecutionError
                    {
                        WorkflowId = workflow.Id,
                        ExecutionPointerId = pointer.Id,
                        ErrorTime = _datetimeProvider.Now.ToUniversalTime(),
                        Message = $"Unable to find step {pointer.StepId} in workflow definition"
                    });
                    continue;
                }

                try
                {
                    if (!InitializeStep(workflow, step, wfResult, def, pointer))
                        continue;

                    await ExecuteStep(workflow, step, pointer, wfResult, def);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        WellKnownLoggingEventIds.WorkflowStepExecutionError,
                        ex,
                        "Workflow {Id} raised error on step {StepId} ({StepName})",
                        workflow.Id,
                        pointer.StepId, pointer.StepName);

                    wfResult.Errors.Add(new ExecutionError
                    {
                        WorkflowId = workflow.Id,
                        ExecutionPointerId = pointer.Id,
                        ErrorTime = _datetimeProvider.Now.ToUniversalTime(),
                        Message = ex.Message
                    });

                    _executionResultProcessor.HandleStepException(workflow, def, pointer, step, ex);
                    _host.ReportStepError(workflow, step, ex);
                }
                _cancellationProcessor.ProcessCancellations(workflow, def, wfResult);
            }
            ProcessAfterExecutionIteration(workflow, def, wfResult);
            DetermineNextExecutionTime(workflow);

            return wfResult;
        }

        private bool InitializeStep(WorkflowInstance workflow, WorkflowStep step, WorkflowExecutorResult wfResult, WorkflowDefinition def, IExecutionPointer pointer)
        {
            switch (step.InitForExecution(wfResult, def, workflow, pointer))
            {
                case ExecutionPipelineDirective.Defer:
                    return false;
                case ExecutionPipelineDirective.EndWorkflow:
                    workflow.Status = WorkflowStatus.Complete;
                    workflow.CompleteTime = _datetimeProvider.Now.ToUniversalTime();
                    return false;
            }

            if (pointer.Status != PointerStatus.Running)
            {
                pointer.Status = PointerStatus.Running;
                _publisher.PublishNotification(new StepStarted
                {
                    EventTimeUtc = _datetimeProvider.Now,
                    Reference = workflow.Reference,
                    ExecutionPointerId = pointer.Id,
                    StepId = step.Id,
                    WorkflowInstanceId = workflow.Id,
                    WorkflowDefinitionId = workflow.WorkflowDefinitionId,
                    Version = workflow.Version,
                    StepExternalId = step.ExternalId
                });
            }

            if (!pointer.StartTime.HasValue)
            {
                pointer.StartTime = _datetimeProvider.Now.ToUniversalTime();
            }

            return true;
        }

        private async Task ExecuteStep(WorkflowInstance workflow, WorkflowStep step, IExecutionPointer pointer, WorkflowExecutorResult wfResult, WorkflowDefinition def)
        {
            using var scope = _scopeProvider.CreateScope();
            
            _logger.LogDebug(
                WellKnownLoggingEventIds.WorkflowStepStarting,
                "Starting step {StepId} ({StepName}) in workflow {DefinitionId} ({InstanceId})",
                step.Id, step.Name, def.Id, workflow.Id);

            IStepBody body = step.ConstructBody(scope.ServiceProvider);

            if (body == null)
            {
                _logger.LogError(
                    WellKnownLoggingEventIds.WorkflowFailedToConstructStepBody,
                    "Unable to construct step body for step {StepId} ({StepName}) and {BodyType} in workflow {DefinitionId} ({InstanceId})",
                    step.Id, step.Name, step.BodyType, def.Id, workflow.Id);

                pointer.SleepUntil = _datetimeProvider.Now.ToUniversalTime().Add(_options.ErrorRetryInterval);
                wfResult.Errors.Add(new ExecutionError
                {
                    WorkflowId = workflow.Id,
                    ExecutionPointerId = pointer.Id,
                    ErrorTime = _datetimeProvider.Now.ToUniversalTime(),
                    Message = $"Unable to construct step body for step {step.Id} ({step.Name}) and {step.BodyType} in workflow {def.Id} ({workflow.Id})"
                });
                return;
            }

            IStepExecutionContext context = new StepExecutionContext
            {
                Workflow = workflow,
                Step = step,
                PersistenceData = pointer.PersistenceData,
                ExecutionPointer = pointer,
                Item = pointer.ContextItem
            };

            foreach (var input in step.Inputs)
                input.AssignInput(workflow.Data, body, context);

            switch (step.BeforeExecute(wfResult, context, pointer, body))
            {
                case ExecutionPipelineDirective.Defer:
                    return;
                case ExecutionPipelineDirective.EndWorkflow:
                    workflow.Status = WorkflowStatus.Complete;
                    workflow.CompleteTime = _datetimeProvider.Now.ToUniversalTime();
                    return;
            }

            var result = await body.RunAsync(context);

            if (result.Proceed)
            {
                foreach (var output in step.Outputs)
                    output.AssignOutput(workflow.Data, body, context);
            }

            _executionResultProcessor.ProcessExecutionResult(workflow, def, pointer, step, result, wfResult);
            step.AfterExecute(wfResult, context, result, pointer);
        }

        private void ProcessAfterExecutionIteration(WorkflowInstance workflow, WorkflowDefinition workflowDef, WorkflowExecutorResult workflowResult)
        {
            var pointers = workflow.ExecutionPointers.Where(x => x.EndTime == null);

            foreach (var pointer in pointers)
            {
                var step = workflowDef.Steps.FindById(pointer.StepId);
                step?.AfterWorkflowIteration(workflowResult, workflowDef, workflow, pointer);
            }
        }

        private void DetermineNextExecutionTime(WorkflowInstance workflow)
        {
            //TODO: move to own class
            workflow.NextExecution = null;

            if (workflow.Status == WorkflowStatus.Complete || workflow.Status == WorkflowStatus.Terminated)
                return;

            foreach (var pointer in workflow.ExecutionPointers.Where(x => x.Active && (x.Children ?? new List<string>()).Count == 0))
            {
                if (!pointer.SleepUntil.HasValue)
                {
                    workflow.NextExecution = 0;
                    return;
                }

                var pointerSleep = pointer.SleepUntil.Value.ToUniversalTime().Ticks;
                workflow.NextExecution = Math.Min(pointerSleep, workflow.NextExecution ?? pointerSleep);
            }

            if (workflow.NextExecution == null)
            {
                foreach (var pointer in workflow.ExecutionPointers.Where(x => x.Active && (x.Children ?? new List<string>()).Count > 0))
                {
                    if (!workflow.ExecutionPointers.FindByScope(pointer.Id).All(x => x.EndTime.HasValue))
                        continue;

                    if (!pointer.SleepUntil.HasValue)
                    {
                        workflow.NextExecution = 0;
                        return;
                    }

                    var pointerSleep = pointer.SleepUntil.Value.ToUniversalTime().Ticks;
                    workflow.NextExecution = Math.Min(pointerSleep, workflow.NextExecution ?? pointerSleep);
                }
            }

            if (workflow.NextExecution != null || workflow.ExecutionPointers.Any(x => x.EndTime == null))
                return;

            workflow.Status = WorkflowStatus.Complete;
            workflow.CompleteTime = _datetimeProvider.Now.ToUniversalTime();
            _publisher.PublishNotification(new WorkflowCompleted
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
