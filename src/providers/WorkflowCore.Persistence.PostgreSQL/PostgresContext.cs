using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkflowCore.Persistence.EntityFramework;
using WorkflowCore.Persistence.EntityFramework.Models;
using WorkflowCore.Persistence.EntityFramework.Services;

namespace WorkflowCore.Persistence.PostgreSQL
{
    public class PostgresContext : WorkflowDbContext
    {
        private readonly string _connectionString;
        private readonly string _tablePrefix;
        private readonly string _schema;

        public PostgresContext(string connectionString,string schema,string tablePrefix)
            :base()
        {
            _schema = schema;
            _tablePrefix = tablePrefix;
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql(_connectionString,
                options =>
                    options.MigrationsHistoryTable(MigrationMetaInfo.MigrationTableName,
                        MigrationMetaInfo.DbSchema));
        }

        protected override void ConfigureSubscriptionStorage(EntityTypeBuilder<PersistedSubscription> builder)
        {
            builder.ToTable(_tablePrefix + "Subscription", _schema);
            builder.Property(x => x.PersistenceId).ValueGeneratedOnAdd();
        }

        protected override void ConfigureWorkflowStorage(EntityTypeBuilder<PersistedWorkflow> builder)
        {
            builder.ToTable(_tablePrefix + "Workflow", _schema);
            builder.Property(x => x.PersistenceId).ValueGeneratedOnAdd();
        }
                
        protected override void ConfigureExecutionPointerStorage(EntityTypeBuilder<PersistedExecutionPointer> builder)
        {
            builder.ToTable(_tablePrefix + "ExecutionPointer", _schema);
            builder.Property(x => x.PersistenceId).ValueGeneratedOnAdd();
        }

        protected override void ConfigureExecutionErrorStorage(EntityTypeBuilder<PersistedExecutionError> builder)
        {
            builder.ToTable(_tablePrefix + "ExecutionError", _schema);
            builder.Property(x => x.PersistenceId).ValueGeneratedOnAdd();
        }

        protected override void ConfigureExetensionAttributeStorage(EntityTypeBuilder<PersistedExtensionAttribute> builder)
        {
            builder.ToTable(_tablePrefix + "ExtensionAttribute", _schema);
            builder.Property(x => x.PersistenceId).ValueGeneratedOnAdd();
        }

        protected override void ConfigureEventStorage(EntityTypeBuilder<PersistedEvent> builder)
        {
            builder.ToTable(_tablePrefix + "Event", _schema);
            builder.Property(x => x.PersistenceId).ValueGeneratedOnAdd();
        }
    }
}

