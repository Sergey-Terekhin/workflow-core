using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WorkflowCore.Interface;

namespace WorkflowCore.Sample14
{
    internal static class Program
    {
        private static async Task Main()
        {
            var serviceProvider = ConfigureServices();

            //start the workflow host
            var host = serviceProvider.GetService<IWorkflowHost>();
            host.RegisterWorkflow<RecurSampleWorkflow, MyData>();
            await host.Start();

            Console.WriteLine("Starting workflow...");
            await host.StartWorkflow("recur-sample");

            Console.ReadLine();
            await host.Stop();
        }

        private static IServiceProvider ConfigureServices()
        {
            //setup dependency injection
            IServiceCollection services = new ServiceCollection();
            services.AddLogging();
            //services.AddWorkflow();
            services.AddWorkflow(/*x => x.UseSqlServer(@"Server=.\SQLEXPRESS;Database=WorkflowCore;Trusted_Connection=True;", true, true)*/);
            
            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}