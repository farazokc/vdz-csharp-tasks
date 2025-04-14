using Onboarding.Interfaces;
using System;

namespace Onboarding.Services
{
    public class ProgressTracker
    {
        private readonly IProgressNotifier _notifier;
        private readonly ILogger _logger;
        public int TotalChunks { get; }
        public int UploadedChunks { get; private set; }
        public string FileName { get; }
        
        public ProgressTracker(int totalChunks, string fileName, IProgressNotifier notifier, ILogger logger)
        {
            TotalChunks = totalChunks;
            FileName = fileName;
            UploadedChunks = 0;
            _notifier = notifier;
            _logger = logger;
        }
        
        public void UpdateProgress(int chunkIndex)
        {
            UploadedChunks++;
            double progressPercentage = GetProgressPercentage();
            
            _logger.Log($"Chunk {chunkIndex + 1}/{TotalChunks} uploaded. Progress: {progressPercentage:F2}%");
            _notifier.NotifyProgress(progressPercentage);
        }
        
        public double GetProgressPercentage()
        {
            return (double)UploadedChunks / TotalChunks * 100;
        }
    }
}