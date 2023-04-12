using System.Collections.Concurrent;

namespace MyBackGroundServices.Controllers
{
    public class TaskQueue : ITaskQueue
    {
        private readonly ConcurrentQueue<TaskInfo> planned = new();
        private readonly ConcurrentDictionary<string, TaskInfo> running = new();
        private readonly ConcurrentDictionary<string, object> taskResults = new();
        private readonly List<LogEntry> logEntries = new();
        private readonly ILogger<TaskQueue> logger;

        public TaskQueue(ILogger<TaskQueue> logger)
        {
            this.logger = logger;
        }

        public bool Enqueue(TaskInfo taskInfo)
        {
            if (planned.Any(m => m.Id == taskInfo.Id) || running.Values.Any(m => m.Id == taskInfo.Id))
            {
                AddLogEntry(taskInfo.Id, "There already is an item with the same id in the queue");
                return false;
            }
            planned.Enqueue(taskInfo);
            return true;
        }

        public void AddResult(string id, object? result)
        {
            if (result != null)
            {
                AddLogEntry(id, $"Result of type {result.GetType()} added to task results.");
                taskResults.AddOrUpdate(id, result, (id, result) => result);
            }
            else
            {
                AddLogEntry(id, $"No result for this task.");
            }
        }

        public TaskInfo? NextTask()
        {
            if (!planned.TryDequeue(out var taskInfo)) return null;
            AddLogEntry(taskInfo.Id, $"Start task {taskInfo.Label}.");
            running.TryAdd(taskInfo.Id, taskInfo);
            return taskInfo;
        }

        public void AddLogEntry(string id, string message)
        {
            logger.LogInformation(message);
            logEntries.Add(new LogEntry(id, message));
        }

        /// <summary>
        /// Clean everything based on the current time minus the time span value.
        /// </summary>
        /// <param name="history"></param>
        /// <returns></returns>
        public int CleanTaskResults(TimeSpan timespan)
        {
            var countRemovals = 0;
            var referenceTime = DateTime.UtcNow.Subtract(timespan);
            List<string> idList = CollectIdForCleanUp(referenceTime);
            foreach (var id in idList)
            {
                if (running.TryRemove(id, out var taskInfo))
                {
                    logger.LogInformation($"Cleaning task {id} - {taskInfo.Label}");
                    countRemovals++;
                }
                if (taskResults.TryRemove(id, out _))
                {
                    logger.LogInformation($"Cleaning task results {id}");
                }
            }
            return countRemovals;
        }

        private List<string> CollectIdForCleanUp(DateTime referenceTime)
        {
            var idList = new List<string>();
            foreach (var taskInfo in running.Values)
            {
                if (taskInfo.ExecutingTask == null) continue;
                if (taskInfo.StartedAtTime >= taskInfo.PlannedAtTime
                    && taskInfo.TaskCompletedAtTime > DateTime.UnixEpoch
                    && taskInfo.TaskCompletedAtTime < referenceTime)
                {
                    logger.LogInformation($"Mark for removal at time: {taskInfo.TaskCompletedAtTime} ");
                    idList.Add(taskInfo.Id);
                }
            }
            return idList;
        }

        public IEnumerable<TaskInfo> All()
        {
            foreach (var item in planned)
            {
                yield return item;
            }
            foreach (var item in running)
            {
                yield return item.Value;
            }
        }

        public IEnumerable<LogEntry> TaskLog() => logEntries;

        public object? GetResult(string id)
        {
            if (taskResults.TryGetValue(id, out var result)) return result;
            return null;
        }

        public bool IsEmpty()
        {
            return planned.Count == 0;
        }

        public int Length()
        {
            return planned.Count;
        }
    }
}