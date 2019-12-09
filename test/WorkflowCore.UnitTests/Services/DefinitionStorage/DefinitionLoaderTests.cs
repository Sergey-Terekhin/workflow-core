using FakeItEasy;
using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.TestAssets.DataTypes;
using Xunit;

namespace WorkflowCore.UnitTests.Services.DefinitionStorage
{
    public class DefinitionLoaderTests
    {

        private readonly IWorkflowProvider _subject;
        private readonly IWorkflowRegistry _registry;

        public DefinitionLoaderTests()
        {
            _registry = A.Fake<IWorkflowRegistry>();
            _subject = new JsonWorkflowProvider.JsonWorkflowProvider();
        }

        [Fact(DisplayName = "Should register workflow")]
        public async Task RegisterDefinition()
        {
            using var reader = new StringReader("{\"Id\": \"HelloWorld\", \"Version\": 1, \"Steps\": []}");
            var definition = await _subject.LoadDefinition(reader);
            _registry.RegisterWorkflow(definition);
            A.CallTo(() => _registry.RegisterWorkflow(A<WorkflowDefinition>.That.Matches(x => x.Id == "HelloWorld"))).MustHaveHappened();
            A.CallTo(() => _registry.RegisterWorkflow(A<WorkflowDefinition>.That.Matches(x => x.Version == 1))).MustHaveHappened();
            A.CallTo(() => _registry.RegisterWorkflow(A<WorkflowDefinition>.That.Matches(x => x.DataType == typeof(object)))).MustHaveHappened();
        }

        [Fact(DisplayName = "Should parse definition")]
        public async Task ParseDefinition()
        {
            using var reader = new StringReader(TestAssets.Utils.GetTestDefinitionJson());
            var definition = await _subject.LoadDefinition(reader);
            _registry.RegisterWorkflow(definition);
            
            A.CallTo(() => _registry.RegisterWorkflow(A<WorkflowDefinition>.That.Matches(x => x.Id == "Test"))).MustHaveHappened();
            A.CallTo(() => _registry.RegisterWorkflow(A<WorkflowDefinition>.That.Matches(x => x.Version == 1))).MustHaveHappened();
            A.CallTo(() => _registry.RegisterWorkflow(A<WorkflowDefinition>.That.Matches(x => x.DataType == typeof(CounterBoard)))).MustHaveHappened();
            A.CallTo(() => _registry.RegisterWorkflow(A<WorkflowDefinition>.That.Matches(MatchTestDefinition, ""))).MustHaveHappened();
        }


        [Fact(DisplayName = "Should parse definition")]
        public async void ParseDefinitionDynamic()
        {
            using var reader = new StringReader(TestAssets.Utils.GetTestDefinitionDynamicJson());
            var definition = await _subject.LoadDefinition(reader);
            _registry.RegisterWorkflow(definition);

            A.CallTo(() => _registry.RegisterWorkflow(A<WorkflowDefinition>.That.Matches(x => x.Id == "Test"))).MustHaveHappened();
            A.CallTo(() => _registry.RegisterWorkflow(A<WorkflowDefinition>.That.Matches(x => x.Version == 1))).MustHaveHappened();
            A.CallTo(() => _registry.RegisterWorkflow(A<WorkflowDefinition>.That.Matches(x => x.DataType == typeof(DynamicData)))).MustHaveHappened();
            A.CallTo(() => _registry.RegisterWorkflow(A<WorkflowDefinition>.That.Matches(MatchTestDefinition, ""))).MustHaveHappened();
        }


        private bool MatchTestDefinition(WorkflowDefinition def)
        {
            //TODO: make this better
            var step1 = def.Steps.Single(s => s.ExternalId == "Step1");
            var step2 = def.Steps.Single(s => s.ExternalId == "Step2");
            
            step1.Outcomes.Count.Should().Be(1);
            step1.Inputs.Count.Should().Be(1);
            step1.Outputs.Count.Should().Be(1);
            step1.Outcomes.Single().NextStep.Should().Be(step2.Id);

            return true;
        }

    }
}
