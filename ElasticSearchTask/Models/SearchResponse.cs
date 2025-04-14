using System.Collections.Generic;

namespace ElasticSearchTask.Models
{
    public class SearchResponse
    {
        public List<LogEntry> Logs { get; set; } = new();
        public long TotalMatches { get; set; }
    }
}