using System;
using System.Collections.Generic;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Primitives
{
    public class Schedule : ContainerStepBody
    {
        public TimeSpan Interval { get; set; }

        /// <inheritdoc />
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            if (context.PersistenceData == null)
            {
                return ExecutionResult.Sleep(Interval, new SchedulePersistenceData { Elapsed = false });
            }
            
            if (context.PersistenceData is SchedulePersistenceData data)
            {
                if (!data.Elapsed)
                {
                    return ExecutionResult.Branch(new List<object> { null }, new SchedulePersistenceData { Elapsed = true });
                }
                
                if (context.Workflow.IsBranchComplete(context.ExecutionPointer.Id))
                {
                    return ExecutionResult.Next();
                }
            
                return ExecutionResult.Persist(context.PersistenceData);
            }
            
            throw new ArgumentException();
        }
    }
}
