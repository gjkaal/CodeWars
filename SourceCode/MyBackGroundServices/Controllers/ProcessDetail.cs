namespace MyBackGroundServices.Controllers
{
    public class ProcessDetail : ProcessStatus
    {
        public int TaskId { get; set; }
        public bool Failed { get; set; }
        public bool IsCompletedSuccessfully { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime StartedAtTime { get; set; }
        public DateTime CompletedAtTime { get; set; }
        public long ExecutionTimeInMilliSeconds { get; internal set; }
        public object? Result { get; internal set; }
    }
}