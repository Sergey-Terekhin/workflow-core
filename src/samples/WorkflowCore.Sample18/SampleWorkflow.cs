using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Sample18
{
    public class SampleWorkflow : IWorkflow<Context>
    {
        public string Id => "WF";
        public int Version => 1;

        public void Build(IWorkflowBuilder<Context> builder)
        {
            builder.StartWith(exec => ExecutionResult.Next()).WaitFor("Event", (data, context) => context.Workflow.Id)
                .Output(data => data.Vars["var1"], step => step.EventData)
                .Then<SomeStep>()
                .Input(step => step.Message, data => data.Vars["var1"])
                .EndWorkflow();
        }
    }
}