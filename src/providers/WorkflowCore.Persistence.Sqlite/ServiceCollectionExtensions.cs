using System;
using System.Linq;
using WorkflowCore.Models;
using WorkflowCore.Persistence.EntityFramework.Services;
using WorkflowCore.Persistence.Sqlite;
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static WorkflowOptions UseSqlite(this WorkflowOptions options, string connectionString, bool canCreateDb)
        {
            options.UsePersistence(sp => new EntityFrameworkPersistenceProvider(new SqliteContextFactory(connectionString), canCreateDb, false));
            return options;
        }
    }
}
