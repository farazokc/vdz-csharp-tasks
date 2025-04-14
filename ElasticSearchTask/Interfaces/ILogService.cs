using ElasticSearchTask.Models;
using System.Threading.Tasks;

namespace ElasticSearchTask.Interfaces
{
    public interface ILogService
    {
        Task IndexLogAsync(LogEntry log);
        Task IndexRandomLogAsync();
        Task RemoveAllLogData();
        Task<SearchResponse> SearchLogsAsync(string query);
    }
}