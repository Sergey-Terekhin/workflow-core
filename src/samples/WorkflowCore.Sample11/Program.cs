using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace WorkflowCore.Sample11
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            IServiceProvider serviceProvider = ConfigureServices();

            //start the workflow host
            var host = serviceProvider.GetService<IWorkflowHost>();
            host.RegisterWorkflow<IfWorkflow, MyData>();
            await host.Start();

            Console.WriteLine("Starting workflow...");
            await host.StartWorkflow("if-sample", new MyData() { Counter = 4 });
            
            Console.ReadLine();
            await host.Stop();
        }

        private static IServiceProvider ConfigureServices()
        {
            //setup dependency injection
            IServiceCollection services = new ServiceCollection();
            services.AddLogging();
            services.AddWorkflow();
            //services.AddWorkflow(x => x.UseMongoDB(@"mongodb://localhost:27017", "workflow-test001"));
            //services.AddWorkflow(x => x.UseSqlServer(@"Server=.\SQLEXPRESS;Database=WorkflowCore;Trusted_Connection=True;", true, true));
            //services.AddWorkflow(x => x.UseSqlite(@"Data Source=database2.db;", true));            


            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}