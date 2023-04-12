using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics;

namespace MyBackGroundServices.Controllers
{
    public class Fork
    {
        public Fork(Func<Task> awaitable, long timeOutInMilliSeconds)
        {
            TimeOutInMilliSeconds = timeOutInMilliSeconds;
            Task.Run(async () =>
            {
                await awaitable.Invoke();
                Completed = true;
            });
        }

        public Fork(Action action, long timeOutInMilliSeconds)
        {
            TimeOutInMilliSeconds = timeOutInMilliSeconds;
            Task.Run(() =>
            {
                action.Invoke();
                Completed = true;
            });
        }

        public bool Completed { get; private set; }
        public long TimeOutInMilliSeconds { get; private set; }

        public async Task JoinAsync(CancellationToken cancellation = new CancellationToken())
        {
            if (Completed) return;
            var t = Stopwatch.StartNew();
            while (!Completed)
            {
                await Task.Delay(10, cancellation).ConfigureAwait(false);
                cancellation.ThrowIfCancellationRequested();
                if (t.ElapsedMilliseconds > TimeOutInMilliSeconds)
                {
                    throw new TimeoutException($"Fork process timed out at {t.ElapsedMilliseconds}ms.");
                }
            }
        }
    }

    public class ProcessHandler : IProcessHandler
    {
        private readonly ITaskQueue taskQueue;
        private readonly IHostApplicationLifetime hostApplicationLifetime;
        private readonly Fork Initializer;

        public ProcessHandler(
            ITaskQueue taskQueue,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            this.taskQueue = taskQueue;
            this.hostApplicationLifetime = hostApplicationLifetime;
            Initializer = new Fork(InitializeAsync, 5000);
        }

        private async Task InitializeAsync()
        {
            await Task.Delay(1000);
        }

        public ProcessDetail GetProcessDetails(string id)
        {
            var task = GetTaskInfo(id);
            if (task == null) return new ProcessDetail { Id = id, Status = "NotFound" };
            var taskStatus = GetTaskStatus(task);
            var runTask = task.ExecutingTask;
            var details = new ProcessDetail
            {
                Id = id,
                Status = taskStatus,
                StartedAtTime = task.StartedAtTime,
                CompletedAtTime = task.TaskCompletedAtTime,
                ExecutionTimeInMilliSeconds = task.ExecutionTimeInMilliSeconds,
                Result = task.TaskResult
            };
            if (runTask != null)
            {
                details.TaskId = runTask.Id;
                details.Failed = runTask.IsFaulted;
                details.IsCompletedSuccessfully = runTask.IsCompletedSuccessfully;
                details.IsCompleted = runTask.IsCompleted;
            }
            return details;
        }

        private static string GetTaskStatus(TaskInfo task)
        {
            var status = "Planned";
            if (task.StartedAtTime >= task.PlannedAtTime) status = "Started";
            if (task.TaskCompletedAtTime >= task.PlannedAtTime)
            {
                var runTask = task.ExecutingTask;
                if (runTask != null)
                {
                    status = runTask.Status.ToString();
                }
                else
                {
                    status = "Completed";
                }
            }
            return status;
        }

        private TaskInfo? GetTaskInfo(string id)
        {
            foreach (var t in taskQueue.All())
            {
                if (t.Id == id) return t;
            }
            return null;
        }

        public IEnumerable<ProcessStatus> Processes()
        {
            var result = new List<ProcessStatus>();
            foreach (var taskInfo in taskQueue.All())
            {
                result.Add(new ProcessStatus
                {
                    Id = taskInfo.Id,
                    Status = GetTaskStatus(taskInfo)
                });
            }
            return result;
        }

        public string Start(string label, Func<CancellationToken, Task<object?>> task)
        {
            if (task == null) return string.Empty;
            var taskInfo = new TaskInfo(label, task);
            taskQueue.Enqueue(taskInfo);
            return taskInfo.Id;
        }

        public T? GetTaskResult<T>(string id)
        {
            var result = taskQueue.GetResult(id);

            return (result != null)
                ? (T)result
                : default;
        }

        public async Task<bool> InitializationComplete()
        {
            await Initializer.JoinAsync();
            return Initializer.Completed;
        }
    }
}