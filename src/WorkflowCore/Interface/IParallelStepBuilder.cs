using System;
using WorkflowCore.Primitives;

namespace WorkflowCore.Interface
{
    /// <summary>
    /// Interface to build workflow step which executes actions in parallel
    /// </summary>
    /// <typeparam name="TData">Workflow data type</typeparam>
    /// <typeparam name="TStepBody">Step body type</typeparam>
    public interface IParallelStepBuilder<TData, TStepBody>
        where TStepBody : IStepBody
    {
        /// <summary>
        /// Build branch for parallel execution
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        IParallelStepBuilder<TData, TStepBody> Do(Action<IWorkflowBuilder<TData>> builder);
        
        /// <summary>
        /// Wait for all branches are completed
        /// </summary>
        /// <returns></returns>
        IStepBuilder<TData, Sequence> Join();
    }
}
