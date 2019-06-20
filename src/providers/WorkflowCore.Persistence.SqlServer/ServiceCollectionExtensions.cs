using System;
using WorkflowCore.Models;
using WorkflowCore.Persistence.EntityFramework;
using WorkflowCore.Persistence.EntityFramework.Services;
using WorkflowCore.Persistence.SqlServer;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static WorkflowOptions UseSqlServer(this WorkflowOptions options, string connectionString, bool canCreateDB, bool canMigrateDB, string schema = null)
        {
            var mi = new MigrationMetaInfo(schema ?? "public");
            options.UsePersistence(sp => new EntityFrameworkPersistenceProvider(new SqlContextFactory(connectionString), canCreateDB, canMigrateDB));
            return options;
        }
    }
}
