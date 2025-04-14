using Onboarding.Interfaces;
using System;
using System.Threading.Tasks;

namespace Onboarding.Services
{
    public class RetryPolicy
    {
        public int MaxRetries { get; }
        public int DelayBetweenRetries { get; }
        private readonly ILogger _logger;
        
        public RetryPolicy(int maxRetries = 3, int delayBetweenRetries = 1000, ILogger logger = null)
        {
            MaxRetries = maxRetries;
            DelayBetweenRetries = delayBetweenRetries;
            _logger = logger;
        }
        
        public async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, string operationName = "Operation")
        {
            int attempt = 1;
            
            while (true)
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex)
                {
                    if (attempt > MaxRetries)
                    {
                        _logger?.LogError($"All {MaxRetries} retry attempts failed for {operationName}. Error: {ex.Message}");
                        throw;
                    }
                    
                    _logger?.Log($"Attempt {attempt}/{MaxRetries} failed for {operationName}. Retrying in {DelayBetweenRetries}ms. Error: {ex.Message}");
                    await Task.Delay(DelayBetweenRetries);
                    attempt++;
                }
            }
        }
    }
}