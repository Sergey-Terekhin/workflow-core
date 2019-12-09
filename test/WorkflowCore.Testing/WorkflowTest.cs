using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Testing
{
    // ReSharper disable once InconsistentNaming
    public abstract class WorkflowTest<TWorkflow, TData> : IDisposable
        where TWorkflow : IWorkflow<TData>, new()
        where TData : class, new()
    {
        protected IWorkflowHost Host;
        protected IPersistenceProvider PersistenceProvider;
        protected List<StepError> UnhandledStepErrors = new List<StepError>();

        protected virtual void Setup()
        {
            //setup dependency injection
            IServiceCollection services = new ServiceCollection();
            services.AddLogging(c => c
                .AddConsole()
                .AddDebug()
                .AddFilter(it => true));
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            PersistenceProvider = serviceProvider.GetService<IPersistenceProvider>();
            Host = serviceProvider.GetService<IWorkflowHost>();
            Host.RegisterWorkflow<TWorkflow, TData>();
            Host.OnStepError += Host_OnStepError;
            Host.Start().Wait();
        }

        protected void Host_OnStepError(WorkflowInstance workflow, WorkflowStep step, Exception exception)
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
        }

        public string StartWorkflow(TData data)
        {
            var def = new TWorkflow();
            var workflowId = Host.StartWorkflow(def.Id, data).Result;
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
            return PersistenceProvider.GetSubcriptions(eventName, eventKey, DateTime.MaxValue).Result;
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

        protected TData GetData(string workflowId)
        {
            var instance = PersistenceProvider.GetWorkflowInstance(workflowId).Result;
            return (TData) instance.Data;
        }

        public void Dispose()
        {
            Host.Stop();
        }
    }

    public class StepError
    {
        public WorkflowInstance Workflow { get; set; }
        public WorkflowStep Step { get; set; }
        public Exception Exception { get; set; }
    }
}