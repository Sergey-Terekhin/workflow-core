using System;
using WorkflowCore.Interface;

namespace WorkflowCore.Services
{
    /// <inheritdoc />
    public class DateTimeProvider : IDateTimeProvider
    {
        /// <inheritdoc />
        public DateTime Now => DateTime.Now;
    }
}
