using System;
using System.Linq;
using WorkflowCore.Models;
using WorkflowCore.Persistence.EntityFramework.Services;
using WorkflowCore.Persistence.PostgreSQL;
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global


namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static WorkflowOptions UsePostgreSQL(this WorkflowOptions options, string connectionString,
            bool canCreateDb, bool canMigrateDb)
        {
            options.UsePersistence(sp =>
                new EntityFrameworkPersistenceProvider(new PostgresContextFactory(connectionString), canCreateDb,
                    canMigrateDb));
            return options;
        }
    }
}
