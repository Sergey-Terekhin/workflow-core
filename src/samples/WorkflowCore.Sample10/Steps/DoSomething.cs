﻿using System;
using WorkflowCore.Interface;
using WorkflowCore.Models;

// ReSharper disable CheckNamespace
namespace WorkflowCore.Sample10
{    
    public class DoSomething : StepBody
    {
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Console.WriteLine("Doing something...");
            return ExecutionResult.Next();
        }
    }
}
