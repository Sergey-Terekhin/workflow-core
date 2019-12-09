using System;

namespace WorkflowCore.Exceptions
{
    /// <summary>
    /// Exception thrown when <see cref="WorkflowCore.Interface.IStepExecutionContext.PersistenceData"/> is corrupted
    /// </summary>
    public class CorruptPersistenceDataException : WorkflowBaseException
    {
    }
}
