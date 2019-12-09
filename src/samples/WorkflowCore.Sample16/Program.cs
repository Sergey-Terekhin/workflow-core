using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace WorkflowCore.Sample16
{
    class Program
    {
        static async Task Main()
        {
            var serviceProvider = ConfigureServices();

            //start the workflow host
            var host = serviceProvider.GetService<IWorkflowHost>();
            host.RegisterWorkflow<ScheduleWorkflow>();
            await host.Start();

            Console.WriteLine("Starting workflow...");
            await host.StartWorkflow("schedule-sample");

            Console.ReadLine();
            await host.Stop();
        }

        private static IServiceProvider ConfigureServices()
        {
            //setup dependency injection
            IServiceCollection services = new ServiceCollection();
            services.AddWorkflow();

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
