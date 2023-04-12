namespace MyBackGroundServices.Controllers
{
    public class LogEntry
    {
        public DateTime Time { get; set; }
        public string TaskId { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;

        public LogEntry()
        {
        }

        public LogEntry(string id, string data)
        {
            Time = DateTime.UtcNow;
            TaskId = id;
            Data = data;
        }
    }
}