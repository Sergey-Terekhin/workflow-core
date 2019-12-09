using System;

namespace WorkflowCore.Interface
{
    /// <summary>
    /// Service to return current date
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Returns current local date and time
        /// </summary>
        DateTime Now { get; }
    }
}