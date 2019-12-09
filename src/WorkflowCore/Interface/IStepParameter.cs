namespace WorkflowCore.Interface
{
    /// <summary>
    /// Interface for step parameter
    /// </summary>
    public interface IStepParameter
    {
        /// <summary>
        /// Assign data to the step's input
        /// </summary>
        /// <param name="data">Workflow instance data</param>
        /// <param name="body">Instance of step body</param>
        /// <param name="context">Execution context</param>
        void AssignInput(object data, IStepBody body, IStepExecutionContext context);
        
        /// <summary>
        /// Assign data to the step's output
        /// </summary>
        /// <param name="data">Workflow instance data</param>
        /// <param name="body">Instance of step body</param>
        /// <param name="context">Execution context</param>
        void AssignOutput(object data, IStepBody body, IStepExecutionContext context);
    }
}