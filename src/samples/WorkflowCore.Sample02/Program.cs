using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WorkflowCore.Interface;


namespace WorkflowCore.Sample02
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IServiceProvider serviceProvider = ConfigureServices();

            //start the workflow host
            var host = serviceProvider.GetService<IWorkflowHost>();
            host.RegisterWorkflow<SimpleDecisionWorkflow>();
            await host.Start();

            await host.StartWorkflow("Simple Decision Workflow");

            Console.ReadLine();
            await host.Stop();
        }

        private static IServiceProvider ConfigureServices()
        {
            //setup dependency injection
            IServiceCollection services = new ServiceCollection();
            services.AddLogging(c => c.AddConsole());
            services.AddWorkflow();
            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}