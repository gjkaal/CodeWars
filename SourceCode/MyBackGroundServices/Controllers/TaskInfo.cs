namespace MyBackGroundServices.Controllers
{
    public class TaskInfo
    {
        public TaskInfo(string label, Func<CancellationToken, Task<object?>> task)
        {
            Id = Guid.NewGuid().ToString();
            Label = label;
            plannedTask = task;
            PlannedAtTime = DateTime.UtcNow;
        }

        public string Id { get; }
        public string Label { get; set; }
        private readonly Func<CancellationToken, Task<object?>> plannedTask;
        public Task<object?>? ExecutingTask { get; private set; }
        public object? TaskResult { get; private set; }
        public DateTime PlannedAtTime { get; private set; }
        public DateTime StartedAtTime { get; private set; }
        public DateTime TaskCompletedAtTime { get; private set; }
        public long ExecutionTimeInMilliSeconds { get; private set; }

        public Task<object?> SetStarting(CancellationToken cancellationToken)
        {
            ExecutingTask = plannedTask.Invoke(cancellationToken);
            StartedAtTime = DateTime.UtcNow;
            return ExecutingTask;
        }

        public void SetCompleted(long executionTime, object? result)
        {
            TaskCompletedAtTime = DateTime.UtcNow;
            ExecutionTimeInMilliSeconds = executionTime;
            TaskResult = result;
        }
    }
}