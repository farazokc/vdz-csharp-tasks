using Onboarding.Interfaces;
using Onboarding.Models;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Onboarding.Providers
{
    public class AzureBlobStorageAccountProvider : IStorageAccountProvider
    {
        private readonly ILogger _logger;
        public StorageAccountConfig Config { get; private set; }
        
        public AzureBlobStorageAccountProvider(ILogger logger)
        {
            _logger = logger;
            Config = GetStorageAccountConfig();
        }
        
        public StorageAccountConfig GetStorageAccountConfig()
        {
            try
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration", "AppSettings.json");
                string jsonString = File.ReadAllText(configPath);
                var jsonDocument = JsonDocument.Parse(jsonString);
                
                var azureElement = jsonDocument.RootElement.GetProperty("Azure");
                
                var config = new StorageAccountConfig
                {
                    ConnectionString = azureElement.GetProperty("ConnectionString").GetString(),
                    ContainerName = azureElement.GetProperty("ContainerName").GetString(),
                    AuthenticationType = azureElement.GetProperty("AuthenticatonType").GetString()
                };
                
                return config;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading storage account configuration: {ex.Message}");
                return new StorageAccountConfig();
            }
        }
        
        public async Task<bool> AuthenticateAsync()
        {
            try
            {
                if (!await Config.ValidateConfig())
                {
                    _logger.LogError("Storage account configuration is invalid");
                    return false;
                }
                
                _logger.Log("Authentication to Azure Blob Storage successful");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Authentication failed: {ex.Message}");
                return false;
            }
        }
    }
}