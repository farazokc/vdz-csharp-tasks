using Azure.Storage.Blobs;
using System;
using System.Threading.Tasks;

namespace Onboarding.Models
{
    public class StorageAccountConfig
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
        public string AuthenticationType { get; set; }
        
        public StorageAccountConfig() { }
        
        public StorageAccountConfig(string connectionString, string containerName, string authenticationType)
        {
            ConnectionString = connectionString;
            ContainerName = containerName;
            AuthenticationType = authenticationType;
        }
        
        public async Task<bool> ValidateConfig()
        {
            try
            {
                if (string.IsNullOrEmpty(ConnectionString) || string.IsNullOrEmpty(ContainerName))
                {
                    return false;
                }

                // Create a BlobServiceClient to test the connection
                var blobServiceClient = new BlobServiceClient(ConnectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);

                // Try to get container properties to verify access
                await containerClient.GetPropertiesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}