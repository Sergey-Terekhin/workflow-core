using System;
using WorkflowCore.Persistence.EntityFramework;
using WorkflowCore.Persistence.EntityFramework.Interfaces;
using WorkflowCore.Persistence.EntityFramework.Services;

namespace WorkflowCore.Persistence.SqlServer
{
    public class SqlContextFactory : IWorkflowDbContextFactory
    {
        private readonly string _connectionString;

        public SqlContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public WorkflowDbContext Build()
        {
            return new SqlServerContext(_connectionString, MigrationMetaInfo.DbSchema,
                MigrationMetaInfo.TableNamePrefix);
        }
    }
}
