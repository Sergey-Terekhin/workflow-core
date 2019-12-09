using System.Threading;
using System.Threading.Tasks;

namespace WorkflowCore.Interface
{
    /// <remarks>
    /// The implementation of this interface will be responsible for
    /// providing a (distributed) locking mechanism to manage in-flight workflows    
    /// </remarks>
    public interface IDistributedLockProvider : IBackgroundTask
    {
        /// <summary>
        /// Try to acquire lock with specified identifier
        /// </summary>
        /// <param name="id">Lock's identifier</param>
        /// <param name="token">Token to cancel operation</param>
        /// <returns></returns>
        Task<bool> AcquireLock(string id, CancellationToken token = default);

        /// <summary>
        /// Releases previously acquired lock
        /// </summary>
        /// <param name="id">Lock's identifier</param>
        /// <returns></returns>
        Task ReleaseLock(string id);
    }
}