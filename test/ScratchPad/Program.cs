using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using WorkflowCore.Interface;
// ReSharper disable UnusedVariable

namespace ScratchPad
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IServiceProvider serviceProvider = ConfigureServices();

            //start the workflow host
            var host = serviceProvider.GetService<IWorkflowHost>();

            host.RegisterWorkflow<WorkflowCore.Sample03.PassingDataWorkflow, WorkflowCore.Sample03.MyDataClass>();
            host.RegisterWorkflow<WorkflowCore.Sample04.EventSampleWorkflow, WorkflowCore.Sample04.MyDataClass>();

            host.Start();
            var data1 = new WorkflowCore.Sample03.MyDataClass() { Value1 = 2, Value2 = 3 };
            host.StartWorkflow("PassingDataWorkflow", data1, "quick dog").Wait();

            var data2 = new WorkflowCore.Sample04.MyDataClass() { Value1 = "test" };
            host.StartWorkflow("EventSampleWorkflow", data2, "alt1 boom").Wait();

            Console.ReadLine();
            host.Stop();
        }

        private static IServiceProvider ConfigureServices()
        {
            //setup dependency injection
            IServiceCollection services = new ServiceCollection();
            services.AddLogging(c=>c.AddConsole().AddDebug());
            //services.AddWorkflow();
            //services.AddWorkflow(x => x.UseMongoDB(@"mongodb://localhost:27017", "workflow"));
            services.AddWorkflow(cfg =>
            {
                //var ddbConfig = new AmazonDynamoDBConfig() { RegionEndpoint = RegionEndpoint.USWest2 };
                //cfg.UseAwsDynamoPersistence(new EnvironmentVariablesAWSCredentials(), ddbConfig, "elastic");
                //cfg.UseAwsSimpleQueueService(new EnvironmentVariablesAWSCredentials(), new AmazonSQSConfig() { RegionEndpoint = RegionEndpoint.USWest2 });
                //cfg.UseAwsDynamoLocking(new EnvironmentVariablesAWSCredentials(), new AmazonDynamoDBConfig() { RegionEndpoint = RegionEndpoint.USWest2 }, "workflow-core-locks");
            });

            services.AddTransient<WorkflowCore.Sample01.Steps.GoodbyeWorld>();

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }

    }
        
}

