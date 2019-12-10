using WorkflowCore.Models;

namespace WorkflowCore.Interface
{
    /// <summary>
    /// Interface for factory to create execution pointers
    /// </summary>
    public interface IExecutionPointerFactory
    {
        /// <summary>
        /// Create execution pointer for newly created workflow
        /// </summary>
        /// <param name="def">Workflow definition</param>
        /// <returns></returns>
        IExecutionPointer BuildGenesisPointer(WorkflowDefinition def);
        
        /// <summary>
        /// Create execution pointer for compensation step
        /// </summary>
        /// <param name="def">Workflow definition</param>
        /// <param name="pointer">Execution pointer to compensate</param>
        /// <param name="exceptionPointer">Execution pointer caused error</param>
        /// <param name="compensationStepId">Identifier of the step used to compensate error</param>
        /// <returns></returns>
        IExecutionPointer BuildCompensationPointer(WorkflowDefinition def, IExecutionPointer pointer, IExecutionPointer exceptionPointer, int compensationStepId);
        
        /// <summary>
        /// Create execution pointer for the next step
        /// </summary>
        /// <param name="def">Workflow definition</param>
        /// <param name="pointer">Current execution pointer</param>
        /// <param name="outcomeTarget"></param>
        /// <returns></returns>
        IExecutionPointer BuildNextPointer(WorkflowDefinition def, IExecutionPointer pointer, StepOutcome outcomeTarget);
        
        /// <summary>
        /// Create child execution pointer for the specified instance
        /// </summary>
        /// <param name="def">Workflow definition (scheme)</param>
        /// <param name="pointer">Parent pointer</param>
        /// <param name="childDefinitionId"></param>
        /// <param name="branch"></param>
        /// <returns></returns>
        IExecutionPointer BuildChildPointer(WorkflowDefinition def, IExecutionPointer pointer, int childDefinitionId, object branch);
    }
}