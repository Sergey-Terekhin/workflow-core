using WorkflowCore.Models;

namespace WorkflowCore.Interface
{
    /// <summary>
    /// Interface for factory to create execution pointers
    /// </summary>
    public interface IExecutionPointerFactory
    {
        IExecutionPointer BuildGenesisPointer(WorkflowDefinition def);
        IExecutionPointer BuildCompensationPointer(WorkflowDefinition def, IExecutionPointer pointer, IExecutionPointer exceptionPointer, int compensationStepId);
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