using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Services.BackgroundTasks
{
    internal class RunnablePoller : IBackgroundTask
    {
        private readonly IPersistenceProvider _persistenceStore;
        private readonly IDistributedLockProvider _lockProvider;
        private readonly IQueueProvider _queueProvider;
        private readonly ILogger _logger;
        private readonly WorkflowOptions _options;
        private Timer _pollTimer;

        public RunnablePoller(IPersistenceProvider persistenceStore, IQueueProvider queueProvider, ILoggerFactory loggerFactory, IDistributedLockProvider lockProvider, WorkflowOptions options)
        {
            _persistenceStore = persistenceStore;
            _queueProvider = queueProvider;
            _logger = loggerFactory.CreateLogger<RunnablePoller>();
            _lockProvider = lockProvider;
            _options = options;
        }

        public Task Start()
        {
            _pollTimer = new Timer(PollRunnables, null, TimeSpan.FromSeconds(0), _options.PollInterval);
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            if (_pollTimer != null)
            {
                _pollTimer.Dispose();
                _pollTimer = null;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Poll the persistence store for workflows ready to run.
        /// Poll the persistence store for stashed unpublished events
        /// </summary>        
        private async void PollRunnables(object target)
        {
            try
            {
                const string pollRunnablesLock = "poll_runnables";
                if (await _lockProvider.AcquireLock(pollRunnablesLock))
                {
                    try
                    {
                        _logger.LogDebug(WellKnownLoggingEventIds.WorkflowPollingRunnable,
                            "Polling for runnable workflows");

                        var runnables = await _persistenceStore.GetRunnableInstances(DateTime.Now);
                        foreach (var item in runnables)
                        {
                            _logger.LogDebug(WellKnownLoggingEventIds.WorkflowFoundRunnable, 
                                "Got runnable instance {WorkflowId}",
                                item);
                            await _queueProvider.QueueWork(item, QueueType.Workflow);
                        }
                    }
                    finally
                    {
                        await _lockProvider.ReleaseLock(pollRunnablesLock);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    WellKnownLoggingEventIds.WorkflowFailedToPollRunnable,
                    ex,
                    "Failed to poll runnable workflows");
            }

            try
            {
                const string unprocessedEventsLock = "unprocessed_events";
                if (await _lockProvider.AcquireLock(unprocessedEventsLock))
                {
                    try
                    {
                        _logger.LogDebug(WellKnownLoggingEventIds.EventPollingUnprocessed, "Polling for unprocessed events");
                        var events = await _persistenceStore.GetRunnableEvents(DateTime.Now);
                        foreach (var item in events.ToList())
                        {
                            _logger.LogDebug(WellKnownLoggingEventIds.EventFoundUnprocessed, 
                                "Got unprocessed event {EventId}", 
                                item);

                            await _queueProvider.QueueWork(item, QueueType.Event);
                        }
                    }
                    finally
                    {
                        await _lockProvider.ReleaseLock(unprocessedEventsLock);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    WellKnownLoggingEventIds.EventFailedToGetUnprocessed,
                    ex,
                    "Failed to get unprocessed event");
            }
        }
    }
}
