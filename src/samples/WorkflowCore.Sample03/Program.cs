using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkflowCore.Interface;


namespace WorkflowCore.Sample03
{
    public class Program
    {
        public static async Task Main()
        {
            IServiceProvider serviceProvider = ConfigureServices();

            //start the workflow host
            var host = serviceProvider.GetService<IWorkflowHost>();
            host.RegisterWorkflow<PassingDataWorkflow, MyDataClass>();
            host.RegisterWorkflow<PassingDataWorkflow2, Dictionary<string, int>>();
            await host.Start();

            //host.StartWorkflow("PassingDataWorkflow", 1, initialData);


            var initialData2 = new Dictionary<string, int>
            {
                ["Value1"] = 7,
                ["Value2"] = 2
            };

            await host.StartWorkflow("PassingDataWorkflow2", 1, initialData2);

            Console.ReadLine();
            await host.Stop();
        }

        private static IServiceProvider ConfigureServices()
        {
            //setup dependency injection
            IServiceCollection services = new ServiceCollection();
            services.AddLogging(c=>c.AddConsole());
            services.AddWorkflow();
            //services.AddWorkflow(x => x.UseSqlServer(@"Server=.\SQLEXPRESS;Database=WorkflowCore;Trusted_Connection=True;", true, true));
            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
