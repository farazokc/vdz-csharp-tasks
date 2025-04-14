using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Onboarding.Interfaces;
using Onboarding.Models;
using Onboarding.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Onboarding.Services
{
    public class FileUploader
    {
        private readonly int _chunkSize;
        private readonly StorageAccountConfig _config;
        private readonly IProgressNotifier _progressNotifier;
        private readonly ILogger _logger;
        private readonly RetryPolicy _retryPolicy;
        
        public FileUploader(
            StorageAccountConfig config,
            IProgressNotifier progressNotifier,
            ILogger logger,
            int? chunkSizeInMb = null)
        {
            _config = config;
            _progressNotifier = progressNotifier;
            _logger = logger;
            _retryPolicy = new RetryPolicy(logger: logger);
            
            // Get chunk size from config if not provided
            if (chunkSizeInMb == null)
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration", "AppSettings.json");
                string jsonString = File.ReadAllText(configPath);
                var jsonDocument = JsonDocument.Parse(jsonString);
                
                chunkSizeInMb = jsonDocument.RootElement.GetProperty("Chunk")
                    .GetProperty("DefaultChunkSize").GetInt32();
            }
            
            // Convert MB to bytes (1MB = 1048576 bytes)
            _chunkSize = chunkSizeInMb.Value * 1024 * 1024;
        }
        
        public List<Chunk> SplitFile(string filePath)
        {
            var chunks = new List<Chunk>();
            long fileSize = new FileInfo(filePath).Length;
            
            for (int i = 0; i < fileSize; i += _chunkSize)
            {
                int endByte = (int)Math.Min(i + _chunkSize - 1, fileSize - 1);
                chunks.Add(new Chunk(filePath, i, endByte, chunks.Count));
            }
            
            return chunks;
        }
        
        public async Task<bool> UploadChunksAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.LogError($"File not found: {filePath}");
                    return false;
                }
                
                var chunks = SplitFile(filePath);
                string fileName = Path.GetFileName(filePath);
                
                _logger.Log($"Starting upload of {fileName} in {chunks.Count} chunks");
                
                var progressTracker = new ProgressTracker(chunks.Count, fileName, _progressNotifier, _logger);
                
                // Create blob client
                var blobServiceClient = new BlobServiceClient(_config.ConnectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(_config.ContainerName);
                
                // Create container if it doesn't exist
                await containerClient.CreateIfNotExistsAsync();
                
                // Create a block blob client
                var blobClient = containerClient.GetBlockBlobClient(fileName);
                
                // List for block IDs
                var blockIds = new List<string>();
                
                // Upload each chunk
                for (int i = 0; i < chunks.Count; i++)
                {
                    var chunk = chunks[i];
                    
                    // Create a base64 encoded block ID
                    string blockId = Convert.ToBase64String(
                        System.Text.Encoding.UTF8.GetBytes(string.Format("BlockId{0}", i.ToString("d6"))));
                    
                    await _retryPolicy.ExecuteWithRetryAsync(async () =>
                    {
                        using (MemoryStream ms = new MemoryStream(chunk.GetChunkData()))
                        {
                            await blobClient.StageBlockAsync(blockId, ms);
                        }
                        return true;
                    }, $"Upload chunk {i + 1}/{chunks.Count}");
                    
                    blockIds.Add(blockId);
                    progressTracker.UpdateProgress(i);
                }
                
                // Commit all blocks
                await blobClient.CommitBlockListAsync(blockIds);
                
                _logger.Log($"Upload completed for {fileName}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Upload failed: {ex.Message}");
                return false;
            }
        }
    }
}