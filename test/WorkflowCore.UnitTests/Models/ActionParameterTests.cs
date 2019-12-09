using System;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using Xunit;
using FluentAssertions;

// ReSharper disable UnusedAutoPropertyAccessor.Local

// ReSharper disable once CheckNamespace
namespace WorkflowCore.UnitTests
{
    public class ActionParameterTests
    {
        [Fact]
        public void should_assign_input()
        {
            var subject = new ActionParameter<MyStep, MyData>((body, item) => { body.Value1 = item.Value1; });
            var data = new MyData
            {
                Value1 = 5
            };
            var step = new MyStep();

            subject.AssignInput(data, step, new StepExecutionContext());

            step.Value1.Should().Be(data.Value1);
        }

        [Fact]
        public void should_assign_output()
        {
            var subject = new ActionParameter<MyStep, MyData>((body, item) => { item.Value1 = body.Value1; });
            var data = new MyData();
            var step = new MyStep()
            {
                Value1 = 5
            };

            subject.AssignOutput(data, step, new StepExecutionContext());

            data.Value1.Should().Be(step.Value1);
        }

        [Fact]
        public void should_convert_input()
        {
            var subject = new ActionParameter<MyStep, MyData>((body, item) => { body.Value2 = item.Value1; });

            var data = new MyData()
            {
                Value1 = 5
            };

            var step = new MyStep();

            subject.AssignInput(data, step, new StepExecutionContext());

            step.Value2.Should().Be(data.Value1);
        }

        [Fact]
        public void should_convert_output()
        {
            var subject = new ActionParameter<MyStep, MyData>((body, item) => { item.Value2 = body.Value1; });

            var data = new MyData()
            {
                Value1 = 5
            };

            var step = new MyStep();

            subject.AssignOutput(data, step, new StepExecutionContext());

            data.Value2.Should().Be(step.Value1);
        }


        class MyData
        {
            public int Value1 { get; set; }
            public object Value2 { get; set; }
        }

        class MyStep : IStepBody
        {
            public int Value1 { get; set; }
            public object Value2 { get; set; }

            public Task<ExecutionResult> RunAsync(IStepExecutionContext context)
            {
                throw new NotImplementedException();
            }
        }
    }
}