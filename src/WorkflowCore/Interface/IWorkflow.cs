namespace WorkflowCore.Interface
{
    /// <summary>
    /// Interface for typed workflow
    /// </summary>
    /// <typeparam name="TData">Workflow data type</typeparam>
    public interface IWorkflow<TData>
        where TData : new()
    {
        /// <summary>
        /// Identifier of workflow
        /// </summary>
        string Id { get; }
        /// <summary>
        /// Version of workflow
        /// </summary>
        int Version { get; }
        
        /// <summary>
        /// Compose workflow from separate steps
        /// </summary>
        /// <param name="builder">instance of workflow builder</param>
        void Build(IWorkflowBuilder<TData> builder);
    }

    /// <summary>
    /// Interface for non-typed workflow
    /// </summary>
    public interface IWorkflow : IWorkflow<object>
    {
    }
}
