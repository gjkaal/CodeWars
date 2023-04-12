using System.Collections.Concurrent;

namespace MyBackGroundServices.Controllers;

/// <summary>
/// Abstraction for a task storage.
/// </summary>
public interface ITaskQueue
{
    /// <summary>
    /// Add a new task to the task queue.
    /// </summary>
    /// <param name="taskInfo">The tasks that should be added.</param>
    bool Enqueue(TaskInfo taskInfo);

    /// <summary>
    /// Get a list of all planned and executing tasks
    /// </summary>
    IEnumerable<TaskInfo> All();

    /// <summary>
    /// Check if there are still tasks in the planned task list.
    /// If there is a task, it will be marked as 'running' and
    /// returned. If no unexecuted tasks are found, an empty value
    /// is returned (null).
    ///
    /// A task cannot be requeued with the same Id.
    /// </summary>
    /// <returns></returns>
    TaskInfo? NextTask();

    /// <summary>
    /// Add the result to the task result set.
    /// It will be kept for a limited amount of time.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="result"></param>
    void AddResult(string id, object? result);

    /// <summary>
    /// Return the log entries that are logged for the planned tasks
    /// </summary>
    /// <returns></returns>
    IEnumerable<LogEntry> TaskLog();

    /// <summary>
    /// Add an entry to the task log. The Id should match a
    /// task. Guid.Empty is allowed for generic log entries,
    /// if they are related to the tasks.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="message"></param>
    void AddLogEntry(string id, string message);

    /// <summary>
    /// Try to find a task result in the data store.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    object? GetResult(string id);

    /// <summary>
    /// True if thare are no waiting tasks in the queue/
    /// </summary>
    /// <returns></returns>
    bool IsEmpty();

    /// <summary>
    /// Retunr the current waiting queue size.
    /// </summary>
    /// <returns></returns>
    int Length();

    /// <summary>
    /// Clean everything based on the current time minus the time span value.
    /// </summary>
    /// <param name="history"></param>
    /// <returns>Number of removed completed results.</returns>
    int CleanTaskResults(TimeSpan timespan);
}