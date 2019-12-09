using System;
using System.Collections.Generic;
using System.Linq;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Services
{
    /// <inheritdoc />
    public class ExecutionPointerFactory : IExecutionPointerFactory
    {
        /// <inheritdoc />
        public IExecutionPointer BuildGenesisPointer(WorkflowDefinition def)
        {
            return new ExecutionPointer
            {
                Id = GenerateId(),
                StepId = 0,
                Active = true,
                Status = PointerStatus.Pending,
                StepName = def.Steps.FindById(0).Name
            };
        }

        /// <inheritdoc />
        public IExecutionPointer BuildNextPointer(WorkflowDefinition def, IExecutionPointer pointer, StepOutcome outcomeTarget)
        {
            var nextId = GenerateId();
            return new ExecutionPointer
            {
                Id = nextId,
                PredecessorId = pointer.Id,
                StepId = outcomeTarget.NextStep,
                Active = true,
                ContextItem = pointer.ContextItem,
                Status = PointerStatus.Pending,
                StepName = def.Steps.FindById(outcomeTarget.NextStep).Name,
                Scope = new List<string>(pointer.Scope)
            };            
        }

        /// <inheritdoc />
        public IExecutionPointer BuildChildPointer(WorkflowDefinition def, IExecutionPointer pointer, int childDefinitionId, object branch)
        {
            var childPointerId = GenerateId();
            var childScope = new List<string>(pointer.Scope);
            childScope.Insert(0, pointer.Id);
            pointer.Children.Add(childPointerId);

            return new ExecutionPointer
            {
                Id = childPointerId,
                PredecessorId = pointer.Id,
                StepId = childDefinitionId,
                Active = true,
                ContextItem = branch,
                Status = PointerStatus.Pending,
                StepName = def.Steps.FindById(childDefinitionId).Name,
                Scope = new List<string>(childScope)
            };            
        }

        /// <inheritdoc />
        public IExecutionPointer BuildCompensationPointer(WorkflowDefinition def, IExecutionPointer pointer, IExecutionPointer exceptionPointer, int compensationStepId)
        {
            var nextId = GenerateId();
            return new ExecutionPointer
            {
                Id = nextId,
                PredecessorId = exceptionPointer.Id,
                StepId = compensationStepId,
                Active = true,
                ContextItem = pointer.ContextItem,
                Status = PointerStatus.Pending,
                StepName = def.Steps.FindById(compensationStepId).Name,
                Scope = new List<string>(pointer.Scope)
            };
        }

        private string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
