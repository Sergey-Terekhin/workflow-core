using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using WorkflowCore.Models;
using WorkflowCore.Providers.Azure.Services;
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static WorkflowOptions UseAzureSyncronization(this WorkflowOptions options, string connectionString)
        {
            options.UseQueueProvider(sp => new AzureStorageQueueProvider(connectionString));
            options.UseDistributedLockManager(sp => new AzureLockManager(connectionString, sp.GetService<ILoggerFactory>()));
            return options;
        }
    }
}
