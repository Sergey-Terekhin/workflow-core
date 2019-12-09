using System;
using System.Linq;
using WorkflowCore.Interface;
using WorkflowCore.Sample06.Steps;

namespace WorkflowCore.Sample06
{
    public class MultipleOutcomeWorkflow : IWorkflow
    {
        public string Id => "MultipleOutcomeWorkflow";

        public int Version => 1;

        public void Build(IWorkflowBuilder<object> builder)
        {
            builder
                .StartWith<RandomOutput>(x => x.Name("Random Step"))
                .When(it => 0)
                    .Do(branch1 => branch1
                        .StartWith<TaskA>()
                        .Then<TaskB>())
                .When(it => 1)
                    .Do(branch2 => branch2
                        .StartWith<TaskC>()
                        .Then<TaskD>())
                .End<RandomOutput>("Random Step");
        }
    }
}