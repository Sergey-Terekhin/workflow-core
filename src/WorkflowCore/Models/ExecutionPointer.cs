using System;
using System.Collections.Generic;
using System.Linq;
using WorkflowCore.Interface;

namespace WorkflowCore.Models
{
    /// <inheritdoc />
    public class ExecutionPointer : IExecutionPointer
    {
        private IReadOnlyCollection<string> _scope = new List<string>();

        internal ExecutionPointer()
        {
        }

        /// <summary>
        /// creates new instance of <see cref="IExecutionPointer"/>
        /// </summary>
        /// <returns></returns>
        public static IExecutionPointer Create() => new ExecutionPointer();

        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        public int StepId { get; set; }

        /// <inheritdoc />
        public bool Active { get; set; }

        /// <inheritdoc />
        public DateTime? SleepUntil { get; set; }

        /// <inheritdoc />
        public object PersistenceData { get; set; }

        /// <inheritdoc />
        public DateTime? StartTime { get; set; }

        /// <inheritdoc />
        public DateTime? EndTime { get; set; }

        /// <inheritdoc />
        public string EventName { get; set; }

        /// <inheritdoc />
        public string EventKey { get; set; }

        /// <inheritdoc />
        public bool EventPublished { get; set; }

        /// <inheritdoc />
        public object EventData { get; set; }

        /// <inheritdoc />
        public Dictionary<string, object> ExtensionAttributes { get; set; } = new Dictionary<string, object>();

        /// <inheritdoc />
        public string StepName { get; set; }

        /// <inheritdoc />
        public int RetryCount { get; set; }

        /// <inheritdoc />
        public List<string> Children { get; set; } = new List<string>();

        /// <inheritdoc />
        public object ContextItem { get; set; }

        /// <inheritdoc />
        public string PredecessorId { get; set; }

        /// <inheritdoc />
        public object Outcome { get; set; }

        /// <inheritdoc />
        public PointerStatus Status { get; set; } = PointerStatus.Legacy;

        /// <inheritdoc />
        public IReadOnlyCollection<string> Scope
        {
            get => _scope;
            set => _scope = new List<string>(value);
        }
    }

    /// <summary>
    /// Statuses of execution pointer
    /// </summary>
    public enum PointerStatus
    {
        /// <summary>
        /// Legacy status. Not used
        /// </summary>
        Legacy = 0,
        /// <summary>
        /// Waiting for execution
        /// </summary>
        Pending = 1,
        /// <summary>
        /// Execution is in progress
        /// </summary>
        Running = 2,
        /// <summary>
        /// Execution was completed successfully
        /// </summary>
        Complete = 3,
        /// <summary>
        /// Pause for specific interval is requested
        /// </summary>
        Sleeping = 4,
        /// <summary>
        /// Execution is suspended until specified event is received
        /// </summary>
        WaitingForEvent = 5,
        /// <summary>
        /// Execution was failed because of error
        /// </summary>
        Failed = 6,
        /// <summary>
        /// Execution was compensated
        /// </summary>
        Compensated = 7,
        /// <summary>
        /// Execution was cancelled
        /// </summary>
        Cancelled = 8
    }
}