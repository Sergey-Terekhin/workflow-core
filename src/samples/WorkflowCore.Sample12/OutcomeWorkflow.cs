using System;
using WorkflowCore.Interface;

namespace WorkflowCore.Sample12
{    
    public class OutcomeWorkflow : IWorkflow<MyData>
    {
        public string Id => "outcome-sample";
        public int Version => 1;

        public void Build(IWorkflowBuilder<MyData> builder)
        {
            builder
                .StartWith<SayHello>()
                .Then<DetermineSomething>()
                    .When(data => 1).Do(then => then
                        .StartWith<PrintMessage>()
                            .Input(step => step.Message, data => "Outcome was 1")
                    )
                    .When(data => 2).Do(then => then
                        .StartWith<PrintMessage>()
                            .Input(step => step.Message, data => "Outcome was 2")
                    )                
                .Then<SayGoodbye>();
        }        
    }

    public class MyData
    {
        public int Counter { get; set; }
    }
}
