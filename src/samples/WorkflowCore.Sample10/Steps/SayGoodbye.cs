﻿using System;
using WorkflowCore.Interface;
using WorkflowCore.Models;
// ReSharper disable CheckNamespace
namespace WorkflowCore.Sample10
{    
    public class SayGoodbye : StepBody
    {
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Console.WriteLine("Goodbye");
            return ExecutionResult.Next();
        }
    }
}
