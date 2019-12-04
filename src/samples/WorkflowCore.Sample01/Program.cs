using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using WorkflowCore.Interface;
using WorkflowCore.Sample01.Steps;

namespace WorkflowCore.Sample01
{
    public class Program
    {

        public static void Main(string[] args)
        {
            IServiceProvider serviceProvider = ConfigureServices();

            //start the workflow host
            var host = serviceProvider.GetService<IWorkflowHost>();
            host.RegisterWorkflow<HelloWorldWorkflow>();
            host.Start();

            host.StartWorkflow("HelloWorld");

            Console.ReadLine();
            host.Stop();
        }

        private static IServiceProvider ConfigureServices()
        {
            //setup dependency injection
            IServiceCollection services = new ServiceCollection();
            services.AddLogging(b =>
            {
                b.AddConsole(o => o.IncludeScopes = true);
                b.AddDebug();
            });
            services.AddWorkflow();
            services.AddTransient<GoodbyeWorld>();

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }


    }
}
