using System;
using WorkflowCore.Models;
using WorkflowCore.Persistence.EntityFramework.Services;
using WorkflowCore.Persistence.SqlServer;

// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static WorkflowOptions UseSqlServer(this WorkflowOptions options, string connectionString, bool canCreateDb, bool canMigrateDb)
        {
            options.UsePersistence(sp => new EntityFrameworkPersistenceProvider(new SqlContextFactory(connectionString), canCreateDb, canMigrateDb));
            return options;
        }
    }
}
