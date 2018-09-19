using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;

namespace WorkflowCore.Sample18
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = ConfigureServices();

            //start the workflow host
            var host = serviceProvider.GetService<IWorkflowHost>();
            host.RegisterWorkflow<SampleWorkflow, Context>();
            host.Start();
            var workflowId = host.StartWorkflow("WF", 1, new Context()).Result;
            Console.WriteLine("Enter value to publish");
            string value = Console.ReadLine();
            host.PublishEvent("Event", workflowId, value);
            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddWorkflow();
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }
    }
}
