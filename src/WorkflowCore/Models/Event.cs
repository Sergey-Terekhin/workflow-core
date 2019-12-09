using System;

namespace WorkflowCore.Models
{
    /// <summary>
    /// Event model which is used by <see cref="Primitives.WaitFor"/> step
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Event identifier
        /// </summary>
        public string Id { get; set; }        

        /// <summary>
        /// Event name
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Event key
        /// </summary>
        public string EventKey { get; set; }

        /// <summary>
        /// Custom event data
        /// </summary>
        public object EventData { get; set; }

        /// <summary>
        /// Event time
        /// </summary>
        public DateTime EventTime { get; set; }

        /// <summary>
        /// Mark event as process
        /// </summary>
        public bool IsProcessed { get; set; }
    }
}
