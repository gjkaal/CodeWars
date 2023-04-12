using System.Diagnostics;

namespace MyBackGroundServices.Controllers
{
    public class TaskQueueBackgroundService : BackgroundService
    {
        private readonly ILogger<TaskQueueBackgroundService> logger;
        private readonly ITaskQueue taskQueue;
        private const int LoopWaitTime = 100;
        public const int WaitTime = 5000;
        private readonly LevelSemaphore pool;

        public TaskQueueBackgroundService(
            ILogger<TaskQueueBackgroundService> logger,
            ITaskQueue taskQueue)
        {
            this.logger = logger;
            this.taskQueue = taskQueue;
            this.pool = new LevelSemaphore(0, 3, nameof(TaskQueueBackgroundService));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("TaskQueueBackgroundService Hosted Service running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var taskId = "BackgroundService";
                var taskQueueEmpty = taskQueue.IsEmpty();
                if (!taskQueueEmpty && pool.CurrentValue < pool.MaxValue)
                {
                    // Not awaiting by design
                    _ = Task.Run(async () =>
                    {
                        if (await pool.WaitOne(TimeSpan.FromSeconds(WaitTime)))
                        {
                            logger.LogInformation($"Task started pool: {pool.CurrentValue}");
                            var taskInfo = taskQueue.NextTask();
                            if (taskInfo != null)
                            {
                                try
                                {
                                    taskId = taskInfo.Id;
                                    var stopWatch = Stopwatch.StartNew();
                                    var executingTask = taskInfo.SetStarting(stoppingToken);
                                    var result = await executingTask.WaitAsync(stoppingToken);
                                    stopWatch.Stop();
                                    taskQueue.AddResult(taskInfo.Id, result);
                                    taskInfo.SetCompleted(stopWatch.ElapsedMilliseconds, result);
                                }
                                catch (Exception e)
                                {
                                    taskQueue.AddLogEntry(taskId, e.Message);
                                }
                            }
                            logger.LogInformation($"Task release pool: {pool.CurrentValue}");
                            pool.Release();
                        }
                    }, stoppingToken);
                }
                else
                {
                    if (taskQueueEmpty)
                        logger.LogInformation($"Task queue is empty, waiting {WaitTime}");
                    else
                        logger.LogInformation($"Currently {taskQueue.Length()} tasks waiting for available thread");
                    if (pool.CurrentValue > 0)
                        logger.LogInformation($"Current active pool = {pool.CurrentValue}");
                    else
                        logger.LogInformation($"No active processes");
                    await Task.Delay(WaitTime);
                }

                var cleaning = taskQueue.CleanTaskResults(TimeSpan.FromMinutes(1));
                if (cleaning > 0)
                {
                    logger.LogInformation($"*** Cleaned {cleaning} completed results");
                }
                await Task.Delay(LoopWaitTime);
            }

            logger.LogInformation("TaskQueueBackgroundService Hosted Service is stopping.");
        }
    }
}