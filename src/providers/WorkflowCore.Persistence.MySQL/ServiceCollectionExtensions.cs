using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using WorkflowCore.Models;
using WorkflowCore.Persistence.EntityFramework.Services;
using WorkflowCore.Persistence.MySQL;
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMethodReturnValue.Global

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static WorkflowOptions UseMySQL(this WorkflowOptions options, string connectionString, bool canCreateDb, bool canMigrateDb, Action<MySqlDbContextOptionsBuilder> mysqlOptionsAction = null)
        {
            options.UsePersistence(sp => new EntityFrameworkPersistenceProvider(new MysqlContextFactory(connectionString, mysqlOptionsAction), canCreateDb, canMigrateDb));
            return options;
        }
    }
}
