using System;

namespace ElasticSearchTask.Models
{
    public class LogEntry
    {
        public LogEntry(){
            Id = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
        }
        public Guid Id { get; set; }
        public required string RandomText { get; set; }
        public DateTime Timestamp { get; set; }
    }
}