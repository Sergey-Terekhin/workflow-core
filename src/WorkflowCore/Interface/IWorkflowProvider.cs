using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WorkflowCore.Models;

namespace WorkflowCore.Interface
{
    /// <summary>
    /// Interface for service to load workflow scheme from external storage
    /// </summary>
    public interface IWorkflowProvider
    {
        /// <summary>
        /// Loads workflow definition from stream
        /// </summary>
        /// <param name="reader">reader containing valid workflow definition</param>
        /// <param name="token">Token to cancel operation</param>
        /// <returns></returns>
        Task<WorkflowDefinition> LoadDefinition(TextReader reader, CancellationToken token = default);
        
    }
}