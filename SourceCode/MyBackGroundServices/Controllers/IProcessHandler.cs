namespace MyBackGroundServices.Controllers
{
    public interface IProcessHandler
    {
        /// <summary>
        /// Retrieve the overview of all current processes.
        /// </summary>
        IEnumerable<ProcessStatus> Processes();

        /// <summary>
        /// Get more details for a single process, inclusing execution information.
        /// </summary>
        /// <param name="id">The process handle</param>
        /// <returns></returns>
        ProcessDetail GetProcessDetails(string id);

        /// <summary>
        /// Start a new task in a process, returning the id that can be used for
        /// information retrieval at a later time.
        /// </summary>
        /// <param name="label">Name for the process that is started</param>
        /// <param name="task">The task that is planned for execution.</param>
        /// <returns></returns>
        string Start(string label, Func<CancellationToken, Task<object?>> task);

        /// <summary>
        /// True if the service is initialized.
        /// </summary>
        /// <returns></returns>
        Task<bool> InitializationComplete();

        /// <summary>
        /// Try to find a result in the completed responses.
        /// </summary>
        /// <param name="id">The process handle</param>
        T? GetTaskResult<T>(string id);
    }
}