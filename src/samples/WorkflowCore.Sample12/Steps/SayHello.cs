﻿using System;
using WorkflowCore.Interface;
using WorkflowCore.Models;
// ReSharper disable CheckNamespace
namespace WorkflowCore.Sample12
{    
    public class SayHello : StepBody
    {
        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Console.WriteLine("Hello");
            return ExecutionResult.Next();
        }
    }
}
