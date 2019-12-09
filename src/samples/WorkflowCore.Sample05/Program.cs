using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace WorkflowCore.Sample05
{
    public class Program
    {
        public static async Task Main()
        {
            IServiceProvider serviceProvider = ConfigureServices();

            //start the workflow host
            var host = serviceProvider.GetService<IWorkflowHost>();
            host.RegisterWorkflow<DeferSampleWorkflow>();
            await host.Start();

            await host.StartWorkflow("DeferSampleWorkflow", 1);

            Console.ReadLine();
            await host.Stop();
        }

        private static IServiceProvider ConfigureServices()
        {
            //setup dependency injection
            IServiceCollection services = new ServiceCollection();
            services.AddLogging(c => c.AddConsole());
            services.AddWorkflow();
            //services.AddWorkflow(x => x.UseSqlServer(@"Server=.;Database=WorkflowCore;Trusted_Connection=True;"));

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}