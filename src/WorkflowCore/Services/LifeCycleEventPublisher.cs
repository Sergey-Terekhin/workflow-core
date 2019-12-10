using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models.LifeCycleEvents;

namespace WorkflowCore.Services
{
    /// <summary>
    /// Implementation of <see cref="ILifeCycleEventPublisher"/>
    /// </summary>
    public class LifeCycleEventPublisher : ILifeCycleEventPublisher, IDisposable
    {
        private readonly ILifeCycleEventHub _eventHub;
        private readonly ILogger _logger;
        private readonly BlockingCollection<LifeCycleEvent> _outbox;
        private Task _dispatchTask;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="eventHub">instance of <see cref="ILifeCycleEventHub"/> to publish events into </param>
        /// <param name="loggerFactory">instance of logger factory</param>
        public LifeCycleEventPublisher(ILifeCycleEventHub eventHub, ILoggerFactory loggerFactory)
        {
            _eventHub = eventHub;
            _outbox = new BlockingCollection<LifeCycleEvent>();
            _logger = loggerFactory.CreateLogger(GetType());
        }

        /// <inheritdoc />
        public void PublishNotification(LifeCycleEvent evt)
        {
            if (_outbox.IsAddingCompleted)
                return;

            _outbox.Add(evt);
        }

        /// <inheritdoc />
        public Task Start()
        {
            if (_dispatchTask != null)
            {
                throw new InvalidOperationException();
            }

            _dispatchTask = Task.Factory.StartNew(Execute, TaskCreationOptions.LongRunning);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task Stop()
        {
            _outbox.CompleteAdding();
            await _dispatchTask;
            _dispatchTask = null;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _outbox.Dispose();
        }

        private async void Execute()
        {
            foreach (var evt in _outbox.GetConsumingEnumerable())
            {
                try
                {
                    await _eventHub.PublishNotification(evt);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        WellKnownLoggingEventIds.EventFailedToPublishLifecycleEvent,
                        ex,
                        "Failed to publish lifecycle event {Event} ({WorkflowInstance} {Reference})",
                        evt.GetType(), evt.WorkflowInstanceId, evt.Reference);
                }
            }
        }
    }
}