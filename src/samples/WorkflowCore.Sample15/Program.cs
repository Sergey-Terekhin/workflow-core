using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using WorkflowCore.Sample15.Steps;
using WorkflowCore.Sample15.Services;

namespace WorkflowCore.Sample15
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            IServiceProvider serviceProvider = ConfigureServices();

            //start the workflow host
            var host = serviceProvider.GetService<IWorkflowHost>();
            host.RegisterWorkflow<HelloWorldWorkflow>();
            await host.Start();

            await host.StartWorkflow("HelloWorld", 1);

            Console.ReadLine();
            await host.Stop();
        }

        private static IServiceProvider ConfigureServices()
        {
            //setup dependency injection
            IServiceCollection services = new ServiceCollection();
            services.AddLogging(c => c.AddDebug().AddFilter(l => true));
            services.AddWorkflow();

            services.AddTransient<DoSomething>();
            services.AddTransient<IMyService, MyService>();

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }

    }
}
