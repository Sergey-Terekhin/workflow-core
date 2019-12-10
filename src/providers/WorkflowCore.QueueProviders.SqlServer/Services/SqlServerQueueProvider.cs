#region using

using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;

using WorkflowCore.Interface;
using WorkflowCore.QueueProviders.SqlServer.Interfaces;

#endregion

namespace WorkflowCore.QueueProviders.SqlServer.Services
{
    public class SqlServerQueueProvider : IQueueProvider
    {
        private readonly string _connectionString;

        private readonly bool _canMigrateDb;
        private readonly bool _canCreateDb;

        private readonly IQueueConfigProvider _config;
        private readonly ISqlServerQueueProviderMigrator _migrator;
        private readonly ISqlCommandExecutor _sqlCommandExecutor;

        private readonly string _queueWorkCommand;
        private readonly string _dequeueWorkCommand;

        public SqlServerQueueProvider(SqlServerQueueProviderOptions opt, IQueueConfigProvider names, ISqlServerQueueProviderMigrator migrator, ISqlCommandExecutor sqlCommandExecutor)
        {
            _config = names;
            _migrator = migrator;
            _sqlCommandExecutor = sqlCommandExecutor;
            _connectionString = opt.ConnectionString;
            _canMigrateDb = opt.CanMigrateDb;
            _canCreateDb = opt.CanCreateDb;

            IsDequeueBlocking = true;

            _queueWorkCommand = GetFromResource("QueueWork");
            _dequeueWorkCommand = GetFromResource("DequeueWork");
        }

        private static string GetFromResource(string file)
        {
            var resName = $"WorkflowCore.QueueProviders.SqlServer.SqlCommands.{file}.sql";
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resName) ?? throw new MissingManifestResourceException(resName);
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }


        public bool IsDequeueBlocking { get; }

#pragma warning disable CS1998

        public async Task Start()
        {
            if (_canCreateDb) _migrator.CreateDb();
            if (_canMigrateDb) _migrator.MigrateDb();
        }

        public async Task Stop()
        {
            // Do nothing
        }

#pragma warning restore CS1998

        public void Dispose()
        {
            Stop().Wait();
        }

        /// <inheritdoc />
        /// <summary>
        /// Write a new id to the specified queue
        /// </summary>
        /// <param name="id"></param>
        /// <param name="queue"></param>
        /// <returns></returns>
        public async Task QueueWork(string id, QueueType queue)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id), "Param id must not be null");

            SqlConnection cn = new SqlConnection(_connectionString);
            try
            {
                cn.Open();
                var par = _config.GetByQueue(queue);

                await _sqlCommandExecutor.ExecuteCommandAsync(cn, null, _queueWorkCommand,
                    CancellationToken.None,
                    new SqlParameter("@initiatorService", par.InitiatorService),
                    new SqlParameter("@targetService", par.TargetService),
                    new SqlParameter("@contractName", par.ContractName),
                    new SqlParameter("@msgType", par.MsgType),
                    new SqlParameter("@RequestMessage", id)
                    );
            }
            finally
            {
                cn.Close();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Get an id from the specified queue.
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>Next id from queue, null if no message arrives in one second.</returns>
        public async Task<string> DequeueWork(QueueType queue, CancellationToken cancellationToken)
        {
            var cn = new SqlConnection(_connectionString);
            try
            {
                await cn.OpenAsync(cancellationToken);
                var par = _config.GetByQueue(queue);                
                var sql = _dequeueWorkCommand.Replace("{queueName}", par.QueueName);
                var msg = await _sqlCommandExecutor.ExecuteScalarAsync<object>(cn, null, sql, cancellationToken);
                return msg is DBNull ? null : (string)msg;
                
            }
            finally
            {
                cn.Close();
            }
        }
    }
}