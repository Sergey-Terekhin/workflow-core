using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace WorkflowCore.QueueProviders.SqlServer.Interfaces
{
    public interface ISqlCommandExecutor
    {
        TResult ExecuteScalar<TResult>(DbConnection cn, DbTransaction tx, string cmdtext, params DbParameter[] parameters);
        int ExecuteCommand(DbConnection cn, DbTransaction tx, string cmdtext, params DbParameter[] parameters);
        
        Task<TResult> ExecuteScalarAsync<TResult>(DbConnection cn, DbTransaction tx, string cmdtext, CancellationToken token = default, params DbParameter[] parameters);
        Task<int> ExecuteCommandAsync(DbConnection cn, DbTransaction tx, string cmdtext, CancellationToken token = default, params DbParameter[] parameters);
    }
}