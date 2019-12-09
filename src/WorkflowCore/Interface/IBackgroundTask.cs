using System.Threading.Tasks;

namespace WorkflowCore.Interface
{
    /// <summary>
    /// <para>Interface for long-running background tasks</para>
    /// <para>Generally, background tasks are started when <see cref="IWorkflowHost"/> is started and stopped when it is stopped</para>
    /// </summary>
    public interface IBackgroundTask
    {
        /// <summary>
        /// Start background task
        /// </summary>
        Task Start();
        
        /// <summary>
        /// Stop background task
        /// </summary>
        Task Stop();
    }
}