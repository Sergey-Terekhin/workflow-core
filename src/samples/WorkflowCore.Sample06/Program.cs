using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace WorkflowCore.Sample06
{
    public class Program
    {
        public static async Task Main()
        {
            IServiceProvider serviceProvider = ConfigureServices();

            //start the workflow host
            var host = serviceProvider.GetService<IWorkflowHost>();
            host.RegisterWorkflow<MultipleOutcomeWorkflow>();
            await host.Start();

            await host.StartWorkflow("MultipleOutcomeWorkflow", 1);

            Console.ReadLine();
            await host.Stop();
        }

        private static IServiceProvider ConfigureServices()
        {
            //setup dependency injection
            IServiceCollection services = new ServiceCollection();
            services.AddLogging(c => c.AddConsole());
            services.AddWorkflow();
            //services.AddWorkflow(x => x.UseSqlServer(@"Server=.;Database=Test3;Trusted_Connection=True;", true, true));            

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}