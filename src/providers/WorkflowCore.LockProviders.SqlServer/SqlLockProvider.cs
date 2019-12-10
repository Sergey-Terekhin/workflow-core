using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WorkflowCore.Interface;
using System.Data;
using System.Collections.Generic;
using System.Threading;

namespace WorkflowCore.LockProviders.SqlServer
{
    public class SqlLockProvider : IDistributedLockProvider
    {
        private const string Prefix = "wfc";

        private readonly string _connectionString;
        private readonly ILogger _logger;
        private readonly Dictionary<string, SqlConnection> _locks = new Dictionary<string, SqlConnection>();
        private readonly AutoResetEvent _mutex = new AutoResetEvent(true);

        public SqlLockProvider(string connectionString, ILoggerFactory logFactory)
        {
            _logger = logFactory.CreateLogger<SqlLockProvider>();
            var csb = new SqlConnectionStringBuilder(connectionString);
            csb.Pooling = true;
            csb.ApplicationName = "Workflow Core Lock Manager";

            _connectionString = csb.ToString();
        }


        public async Task<bool> AcquireLock(string id, CancellationToken token)
        {
            if (_mutex.WaitOne())
            {
                try
                {
                    var connection = new SqlConnection(_connectionString);
                    await connection.OpenAsync(token);
                    try
                    {
                        var cmd = connection.CreateCommand();
                        cmd.CommandText = "sp_getapplock";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Resource", $"{Prefix}:{id}");
                        cmd.Parameters.AddWithValue("@LockOwner", $"Session");
                        cmd.Parameters.AddWithValue("@LockMode", $"Exclusive");
                        cmd.Parameters.AddWithValue("@LockTimeout", 0);
                        var returnParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;
                        await cmd.ExecuteNonQueryAsync(token);
                        var result = Convert.ToInt32(returnParameter.Value);

                        switch (result)
                        {
                            case -1:
                                _logger.LogDebug("The lock request timed out for {Id}", id);
                                break;
                            case -2:
                                _logger.LogDebug("The lock request was canceled for {Id}", id);
                                break;
                            case -3:
                                _logger.LogDebug("The lock request was chosen as a deadlock victim for {Id}", id);
                                break;
                            case -999:
                                _logger.LogError("Lock provider error for {Id}", id);
                                break;
                        }

                        if (result >= 0)
                        {
                            _locks[id] = connection;
                            return true;
                        }
                        else
                        {
                            connection.Close();
                            return false;
                        }
                    }
                    catch (Exception)
                    {
                        connection.Close();
                        throw;
                    }
                }
                finally
                {
                    _mutex.Set();
                }
            }

            return false;
        }

        public async Task ReleaseLock(string id)
        {
            if (_mutex.WaitOne())
            {
                try
                {
                    var connection = _locks[id];

                    if (connection == null)
                        return;

                    try
                    {
                        var cmd = connection.CreateCommand();
                        cmd.CommandText = "sp_releaseapplock";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Resource", $"{Prefix}:{id}");
                        cmd.Parameters.AddWithValue("@LockOwner", $"Session");
                        var returnParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        await cmd.ExecuteNonQueryAsync();
                        var result = Convert.ToInt32(returnParameter.Value);

                        if (result < 0)
                            _logger.LogError("Unable to release lock for {Id}", id);
                    }
                    finally
                    {
                        connection.Close();
                        _locks.Remove(id);
                    }
                }
                finally
                {
                    _mutex.Set();
                }
            }
        }

        public Task Start()=> Task.CompletedTask;

        public Task Stop() => Task.CompletedTask;
    }
}