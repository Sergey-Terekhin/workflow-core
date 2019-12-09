using WorkflowCore.Models;

namespace WorkflowCore.Interface
{
    /// <summary>
    /// Context for step execution
    /// </summary>
    public interface IStepExecutionContext
    {
        /// <summary>
        /// Context item. Has the same value as <see cref="ExecutionPointer"/>.<see cref="IExecutionPointer.ContextItem"/>
        /// </summary>
        object Item { get; set; }

        /// <summary>
        /// Instance of execution pointer
        /// </summary>
        IExecutionPointer ExecutionPointer { get; set; }

        /// <summary>
        /// Persisted data. Has the same value as <see cref="ExecutionPointer"/>.<see cref="IExecutionPointer.PersistenceData"/>
        /// </summary>
        object PersistenceData { get; set; }

        /// <summary>
        /// Step instance
        /// </summary>
        WorkflowStep Step { get; set; }

        /// <summary>
        /// Workflow instance
        /// </summary>
        WorkflowInstance Workflow { get; set; }        
    }
}