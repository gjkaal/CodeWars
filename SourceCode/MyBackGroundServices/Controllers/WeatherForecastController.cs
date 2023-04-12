using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MyBackGroundServices.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LongRunningProcess : ControllerBase
    {
        private readonly IProcessHandler processHandler;
        private readonly ITaskQueue taskQueue;

        public LongRunningProcess(IProcessHandler processHandler, ITaskQueue taskQueue)
        {
            this.processHandler = processHandler;
            this.taskQueue = taskQueue;
        }

        [HttpGet("processes")]
        public IEnumerable<ProcessStatus> Processes() => processHandler.Processes();

        [HttpGet("processDetail/{id}")]
        public ProcessDetail GetProcess(string id) => processHandler.GetProcessDetails(id);

        [HttpGet("processResultA/{id}")]
        public ComplexResultItem? ProcessResult(string id) => processHandler.GetTaskResult<ComplexResultItem>(id);

        [HttpGet("initialized")]
        public Task<bool> ProcessHandlerIntialized() => processHandler.InitializationComplete();

        [HttpPost("triggerProcessA")]
        public string TriggerProcessA() => processHandler.Start(
            nameof(TriggerProcessA),
            async (c) =>
            {
                var t = new Stopwatch();
                t.Start();
                await Task.Delay(5000, c);
                t.Stop();
                return new ComplexResultItem
                {
                    Name = "LogRunningProcess",
                    Description = $"Ran to completion in {t.ElapsedMilliseconds} milliseconds"
                };
            });

        [HttpGet("TaskLog")]
        public IEnumerable<LogEntry> TaskLog() => taskQueue.TaskLog();
    }

    public class ComplexResultItem
    {
        public Guid Uuid { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class LogRunningProcess
    {
        private readonly int timeToRun;

        public LogRunningProcess(int timeToRun)
        {
            this.timeToRun = timeToRun;
        }

        public async Task<object?> Execute(CancellationToken cancellation)
        {
            var t = new Stopwatch();
            t.Start();
            await Task.Delay(timeToRun, cancellation);
            t.Stop();
            return new ComplexResultItem
            {
                Name = "LogRunningProcess",
                Description = $"Ran to completion in {t.ElapsedMilliseconds} milliseconds"
            };
        }
    }
}