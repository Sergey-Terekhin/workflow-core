using System;
using WorkflowCore.Interface;
using WorkflowCore.Models;
// ReSharper disable CheckNamespace
namespace WorkflowCore.Sample12
{    
    public class DetermineSomething : StepBody
    {
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            return ExecutionResult.Outcome(2);
        }
    }
}
