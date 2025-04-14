using ElasticSearchTask.Models;
using System.Threading.Tasks;

namespace ElasticSearchTask.Interfaces
{
    public interface IElasticSearchClient
    {
        Task<bool> IndexDocumentAsync(LogEntry log);
        Task<bool> IndexRandomLogAsync();
        Task<bool> RemoveAllLogsAsync();
        Task<SearchResponse> SearchAsync(string query);
    }
}