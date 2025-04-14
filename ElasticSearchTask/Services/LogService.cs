using ElasticSearchTask.Interfaces;
using ElasticSearchTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchTask.Services
{
    public class LogService(IElasticSearchClient wrapper) : ILogService
    {
        private readonly IElasticSearchClient _elasticClientWrapper = wrapper;

        public async Task IndexLogAsync(LogEntry log)
        {
            if (log == null) throw new ArgumentNullException(nameof(log));

            var response = await _elasticClientWrapper.IndexDocumentAsync(log);
            if (!response)
            {
                throw new Exception("Failed to index log entry");
            }
        }

        public async Task RemoveAllLogDataAsync()
        {
            var response = await _elasticClientWrapper.RemoveAllLogsAsync();
            if (!response)
            {
                throw new Exception("Failed to remove all logs");
            }
        }

        public async Task IndexRandomLogAsync()
        {
            var response = await _elasticClientWrapper.IndexRandomLogAsync();
            if(!response)
            {
                throw new Exception("Failed to index logs");
            }
            return;
        }

        public async Task<SearchResponse> SearchLogsAsync(string query)
        {
            var response = await _elasticClientWrapper.SearchAsync(query);
            if (response == null)
            {
                throw new Exception("Failed to search logs");
            }
            return response;
        }

        public async Task RemoveAllLogData()
        {
            var response = await _elasticClientWrapper.RemoveAllLogsAsync();
            if (!response)
            {
                throw new Exception("Failed to remove all logs");
            }
            return;
        }
    }
}