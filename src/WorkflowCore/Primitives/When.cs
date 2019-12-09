using System;
using System.Linq;
using System.Collections.Generic;
using JetBrains.Annotations;
using WorkflowCore.Exceptions;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Primitives
{
    [PublicAPI]
    public class When : ContainerStepBody
    {
        public object ExpectedOutcome { get; set; }

        /// <inheritdoc />
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            var switchOutcome = GetSwitchOutcome(context);

            if (ExpectedOutcome != switchOutcome)
            {
                if (Convert.ToString(ExpectedOutcome) != Convert.ToString(switchOutcome))
                {
                    return ExecutionResult.Next();
                }
                
            }

            if (context.PersistenceData == null)
            {
                return ExecutionResult.Branch(
                    new List<object> { null }, 
                    new ControlPersistenceData { ChildrenActive = true });
            }

            if (context.PersistenceData is ControlPersistenceData data && data.ChildrenActive)
            {
                if (context.Workflow.IsBranchComplete(context.ExecutionPointer.Id))
                {
                    return ExecutionResult.Next();
                }
                    
                return ExecutionResult.Persist(context.PersistenceData);
            }

            throw new CorruptPersistenceDataException();
        }        

        private object GetSwitchOutcome(IStepExecutionContext context)
        {
            var id = context.ExecutionPointer.Id;
            var switchPointer = context.Workflow.ExecutionPointers.First(x => x.Children.Contains(id));
            return switchPointer.Outcome;
        }
    }
}
