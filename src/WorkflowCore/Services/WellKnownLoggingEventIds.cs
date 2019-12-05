using Microsoft.Extensions.Logging;

namespace WorkflowCore.Services
{
    internal static class WellKnownLoggingEventIds
    {
        public static readonly EventId FailedToAcquireLock = new EventId(0x00010001, nameof(FailedToAcquireLock));
        public static readonly EventId FailedToIndexWorkflow = new EventId(0x00010003, nameof(FailedToIndexWorkflow));
        public static readonly EventId FailedToDequeItem = new EventId(0x00010004, nameof(FailedToDequeItem));
        public static readonly EventId FailedToEnquequeWorkflow = new EventId(0x00010005, nameof(FailedToEnquequeWorkflow));
        public static readonly EventId FailedItemProcessing = new EventId(0x00010006, nameof(FailedItemProcessing));

        public static readonly EventId WorkflowFailedToGetOrUpdate = new EventId(0x00010002, nameof(WorkflowFailedToGetOrUpdate));
        public static readonly EventId WorkflowNotExist = new EventId(0x00010007, nameof(WorkflowNotExist));
        public static readonly EventId WorkflowStepNotExist = new EventId(0x00010008, nameof(WorkflowStepNotExist));
        public static readonly EventId WorkflowStepExecutionError = new EventId(0x00010009, nameof(WorkflowStepExecutionError));
        public static readonly EventId WorkflowStepStarting = new EventId(0x00010010, nameof(WorkflowStepStarting));
        public static readonly EventId WorkflowFailedToConstructStepBody = new EventId(0x00010011, nameof(WorkflowFailedToConstructStepBody));
        public static readonly EventId WorkflowStepCancelConditionExecutionError = new EventId(0x00010012, nameof(WorkflowStepCancelConditionExecutionError));
        public static readonly EventId WorkflowPollingRunnable = new EventId(0x00010013, nameof(WorkflowPollingRunnable));
        public static readonly EventId WorkflowFoundRunnable = new EventId(0x00010014, nameof(WorkflowFoundRunnable));
        public static readonly EventId WorkflowFailedToPollRunnable = new EventId(0x00010015, nameof(WorkflowFailedToPollRunnable));

        public static readonly EventId DebugNewSubscription = new EventId(0x00020000, nameof(DebugNewSubscription));

        public static readonly EventId EventCreateNew = new EventId(0x00020001, nameof(EventCreateNew));
        public static readonly EventId EventFailedToPublishLifecycleEvent = new EventId(0x00020002, nameof(EventFailedToPublishLifecycleEvent));
        public static readonly EventId EventFailedToProcessLifecycleEventSubscriber = new EventId(0x00020003, nameof(EventFailedToProcessLifecycleEventSubscriber));
        public static readonly EventId EventPollingUnprocessed = new EventId(0x00020004, nameof(EventPollingUnprocessed));
        public static readonly EventId EventFoundUnprocessed = new EventId(0x00020005, nameof(EventFoundUnprocessed));
        public static readonly EventId EventFailedToGetUnprocessed = new EventId(0x00020006, nameof(EventFailedToGetUnprocessed));

        public static readonly EventId OperationCancelled = new EventId(0x00030000, nameof(OperationCancelled));
        public static readonly EventId BackgroundTaskStart = new EventId(0x00030001, nameof(BackgroundTaskStart));
        public static readonly EventId BackgroundTaskStopping = new EventId(0x00030002, nameof(BackgroundTaskStopping));
        public static readonly EventId BackgroundTaskStopped = new EventId(0x00030003, nameof(BackgroundTaskStopped));

        
    }
}
