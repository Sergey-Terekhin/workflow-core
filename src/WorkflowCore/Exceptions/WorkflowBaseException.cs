using System;
using System.Linq;

namespace WorkflowCore.Exceptions
{
    /// <summary>
    /// Base type for all workflow exceptions 
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public abstract class WorkflowBaseException : Exception
    {
        /// <summary>
        /// ctor
        /// </summary>
        protected WorkflowBaseException() { }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="inner">Inner exception</param>
        protected WorkflowBaseException(string message, Exception inner = null) : base(message, inner) { }
    }
}