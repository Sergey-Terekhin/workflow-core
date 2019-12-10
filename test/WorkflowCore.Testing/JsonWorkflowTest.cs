using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Testing
{
    public abstract class JsonWorkflowTest : IDisposable
    {
        protected IWorkflowHost Host;
        protected IPersistenceProvider PersistenceProvider;
        protected IWorkflowProvider DefinitionLoader;
        protected IWorkflowRegistry Registry;
        protected List<StepError> UnhandledStepErrors = new List<StepError>();

        protected virtual void Setup()
        {
            //setup dependency injection
            IServiceCollection services = new ServiceCollection();
            services.AddLogging();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            PersistenceProvider = serviceProvider.GetService<IPersistenceProvider>();
            DefinitionLoader = serviceProvider.GetService<IWorkflowProvider>();
            Registry = serviceProvider.GetService<IWorkflowRegistry>();
            Host = serviceProvider.GetService<IWorkflowHost>();
            Host.OnStepError += Host_OnStepError;
            Host.Start().Wait();
        }

        private void Host_OnStepError(WorkflowInstance workflow, WorkflowStep step, Exception exception)
        {
            UnhandledStepErrors.Add(new StepError()
            {
                Exception = exception,
                Step = step,
                Workflow = workflow
            });
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddWorkflow();
            services.AddSingleton<IWorkflowProvider, JsonWorkflowProvider.JsonWorkflowProvider>();
        }

        public async Task<string> StartWorkflow(string json, object data)
        {
            using var reader = new StringReader(json);
            var def = await DefinitionLoader.LoadDefinition(reader);
            Registry.RegisterWorkflow(def);
            var workflowId = await Host.StartWorkflow(def.Id, data);
            return workflowId;
        }

        protected void WaitForWorkflowToComplete(string workflowId, TimeSpan timeOut)
        {
            var status = GetStatus(workflowId);
            var counter = 0;
            while ((status == WorkflowStatus.Runnable) && (counter < (timeOut.TotalMilliseconds / 100)))
            {
                Thread.Sleep(100);
                counter++;
                status = GetStatus(workflowId);
            }
        }

        protected IEnumerable<EventSubscription> GetActiveSubscriptons(string eventName, string eventKey)
        {
            return PersistenceProvider.GetSubscriptions(eventName, eventKey, DateTime.MaxValue).Result;
        }

        protected void WaitForEventSubscription(string eventName, string eventKey, TimeSpan timeOut)
        {
            var counter = 0;
            while ((!GetActiveSubscriptons(eventName, eventKey).Any()) && (counter < (timeOut.TotalMilliseconds / 100)))
            {
                Thread.Sleep(100);
                counter++;
            }
        }

        protected WorkflowStatus GetStatus(string workflowId)
        {
            var instance = PersistenceProvider.GetWorkflowInstance(workflowId).Result;
            return instance.Status;
        }

        protected TData GetData<TData>(string workflowId)
        {
            var instance = PersistenceProvider.GetWorkflowInstance(workflowId).Result;
            return (TData)instance.Data;
        }

        public void Dispose()
        {
            Host.Stop().Wait();
        }
    }
    
}
