using ElasticSearchTask.Interfaces;
using ElasticSearchTask.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElasticSearchTask.Services
{
    public class ElasticClientWrapper : IElasticSearchClient
    {
        private readonly ElasticClient _elasticClient;
        private const string DEFAULT_INDEX = "logs";
        private const int DEFAULT_RESULT_SIZE = 100;
        private const int DEFAULT_RANDOM_LOG_SIZE = 1_000_000;

        public ElasticClientWrapper(string elasticSearchUri)
        {
            var settings = new ConnectionSettings(new Uri(elasticSearchUri))
                .DefaultIndex(DEFAULT_INDEX)
                .DefaultMappingFor<LogEntry>(m => m
                    .IndexName(DEFAULT_INDEX)
                    .IdProperty(l => l.Id)
                );

            _elasticClient = new ElasticClient(settings);

            CreateIndexIfNotExists().Wait();
        }

        private async Task CreateIndexIfNotExists()
        {
            var indexExists = await _elasticClient.Indices.ExistsAsync(DEFAULT_INDEX);
            if (!indexExists.Exists)
            {
                var createIndexResponse = await _elasticClient.Indices.CreateAsync(DEFAULT_INDEX, c => c
                    .Settings(s => s
                        .NumberOfShards(1)
                        .NumberOfReplicas(1))
                    .Map<LogEntry>(m => m
                        .AutoMap()
                        .Properties(ps => ps
                            .Text(t => t
                                .Name(n => n.RandomText)
                                .Analyzer("standard")
                            )
                            .Date(d => d
                                .Name(n => n.Timestamp)
                            )
                        )
                    )
                );

                if (!createIndexResponse.IsValid)
                {
                    throw new Exception($"Failed to create index: {createIndexResponse.DebugInformation}");
                }
            }
        }

        public async Task<bool> RemoveAllLogsAsync()
        {
            var deleteResponse = await _elasticClient.Indices.DeleteAsync(DEFAULT_INDEX);
            if (!deleteResponse.IsValid)
            {
                throw new Exception($"Failed to delete index: {deleteResponse.DebugInformation}");
            }

            // Recreate the index after deletion
            await CreateIndexIfNotExists();

            return deleteResponse.IsValid;
        }

        public async Task<bool> IndexRandomLogAsync()
        {
            try
            {
                const int batchSize = 10000; // Process 10,000 documents per batch
                var timestamp = DateTime.UtcNow;
                int processedCount = 0;

                while (processedCount < DEFAULT_RANDOM_LOG_SIZE)
                {
                    var bulkDescriptor = new BulkDescriptor();
                    var currentBatchSize = Math.Min(batchSize, DEFAULT_RANDOM_LOG_SIZE - processedCount);

                    // Create batch of logs
                    var logs = Enumerable.Range(0, currentBatchSize).Select(_ => new LogEntry
                    {
                        Id = Guid.NewGuid(),
                        RandomText = Faker.NameFaker.Name(),
                        Timestamp = timestamp
                    });

                    // Add operations to bulk descriptor
                    foreach (var log in logs)
                    {
                        bulkDescriptor.Index<LogEntry>(op => op
                            .Document(log)
                            .Index(DEFAULT_INDEX)
                            .Id(log.Id)
                        );
                    }

                    // Execute bulk operation
                    var response = await _elasticClient.BulkAsync(bulkDescriptor);

                    if (!response.IsValid)
                    {
                        throw new Exception($"Bulk insert failed: {response.DebugInformation}");
                    }

                    processedCount += currentBatchSize;
                    Console.WriteLine($"Processed {processedCount:N0} of {DEFAULT_RANDOM_LOG_SIZE:N0} records");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in bulk insert: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> IndexDocumentAsync(LogEntry log)
        {
            if (log.Id == Guid.Empty)
            {
                log.Id = Guid.NewGuid(); // Ensure ID is set if not already
            }

            // Index the log document asynchronously
            var response = await _elasticClient.IndexDocumentAsync(log);

            if (!response.IsValid)
            {
                throw new Exception($"Failed to index document: {response.DebugInformation}");
            }

            return response.IsValid;
        }

        public async Task<SearchResponse> SearchAsync(string query)
        {
            ISearchResponse<LogEntry> response;

            if (string.IsNullOrWhiteSpace(query))
            {
                // Get all documents with a defined constant value
                response = await _elasticClient.SearchAsync<LogEntry>(s => s
                    .Size(DEFAULT_RESULT_SIZE)
                    .Query(q => q
                        .MatchAll()
                    )
                );
            }
            else
            {
                // Execute the search query and return the results
                response = await _elasticClient.SearchAsync<LogEntry>(s => s
                    .Query(q => q
                        .Match(m => m
                            .Field(f => f.RandomText) // Field to search on
                            .Query(query) // Query string
                        )
                    )
                );
            }

            // Map the results to our SearchResponse
            var searchResponse = new SearchResponse
            {
                Logs = response.Documents.ToList(),
                TotalMatches = response.Total
            };

            if (!response.IsValid)
            {
                throw new Exception($"Search failed: {response.DebugInformation}");
            }

            return searchResponse;
        }
    }

}
