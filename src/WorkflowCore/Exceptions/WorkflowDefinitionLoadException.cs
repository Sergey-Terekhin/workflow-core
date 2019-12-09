using System;

namespace WorkflowCore.Exceptions
{
    /// <summary>
    /// Exception thrown if it is impossible to load workflow definition
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class WorkflowDefinitionLoadException : WorkflowBaseException
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="message">Exception message</param>
        public WorkflowDefinitionLoadException(string message)
            : base (message)
        {            
        }
    }
}
