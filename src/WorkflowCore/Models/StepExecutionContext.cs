using WorkflowCore.Interface;

namespace WorkflowCore.Models
{
    /// <inheritdoc />
    public class StepExecutionContext : IStepExecutionContext
    {
        /// <inheritdoc />
        public WorkflowInstance Workflow { get; set; }

        /// <inheritdoc />
        public WorkflowStep Step { get; set; }

        /// <inheritdoc />
        public IExecutionPointer ExecutionPointer { get; set; }

        /// <inheritdoc />
        public object PersistenceData { get; set; }

        /// <inheritdoc />
        public object Item { get; set; }
    }
}
