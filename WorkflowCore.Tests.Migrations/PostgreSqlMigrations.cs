using System;
using Microsoft.EntityFrameworkCore;
using WorkflowCore.Persistence.EntityFramework;
using WorkflowCore.Persistence.PostgreSQL;
using Xunit;

namespace WorkflowCore.Tests.Migrations
{
    public class PostgreSqlMigrations
    {
        private string _connectionString = "Host=10.10.17.107;Port=5432;Database=workflow;Username=postgres;Password=Avanp0st;";
        private string _schema = "wf";

        [Fact]
        public void MigrateUp()
        {
            var mi = new MigrationMetaInfo(_schema);
            var dbContext = new PostgresContext(_connectionString, MigrationMetaInfo.DbSchema,
                MigrationMetaInfo.TableNamePrefix);
            dbContext.Database.Migrate();
            dbContext.Dispose();
        }
    }
}