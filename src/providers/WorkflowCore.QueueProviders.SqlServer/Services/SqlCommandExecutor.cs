using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkflowCore.QueueProviders.SqlServer.Interfaces;

namespace WorkflowCore.QueueProviders.SqlServer.Services
{
    public class SqlCommandExecutor : ISqlCommandExecutor
    {
        public TResult ExecuteScalar<TResult>(DbConnection cn, DbTransaction tx, string cmdtext, params DbParameter[] parameters)
        {
            using (var cmd = cn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = cmdtext;                

                foreach (var param in parameters)
                    cmd.Parameters.Add(param);
                
                return (TResult)cmd.ExecuteScalar();
            }
        }

        public int ExecuteCommand(DbConnection cn, DbTransaction tx, string cmdtext, params DbParameter[] parameters)
        {
            using (var cmd = cn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = cmdtext;

                foreach (var param in parameters)
                    cmd.Parameters.Add(param);

                return cmd.ExecuteNonQuery();
            }
        }

        public async Task<TResult> ExecuteScalarAsync<TResult>(DbConnection cn, DbTransaction tx, string cmdtext, CancellationToken token = default,
            params DbParameter[] parameters)
        {
            using (var cmd = cn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = cmdtext;                

                foreach (var param in parameters)
                    cmd.Parameters.Add(param);
                
                return (TResult)await cmd.ExecuteScalarAsync(token);
            }
        }

        public async Task<int> ExecuteCommandAsync(DbConnection cn, DbTransaction tx, string cmdtext, CancellationToken token = default,
            params DbParameter[] parameters)
        {
            using (var cmd = cn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = cmdtext;

                foreach (var param in parameters)
                    cmd.Parameters.Add(param);

                return await cmd.ExecuteNonQueryAsync(token);
            }
        }
    }
}