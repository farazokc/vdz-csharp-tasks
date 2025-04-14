using Onboarding.Helpers;
using Onboarding.Interfaces;
using Onboarding.Providers;
using Onboarding.Services;
using System;
using System.Threading.Tasks;

namespace Onboarding
{
    class Program
    {
        static async Task Main(string[] args)
        {
            throw new Exception("Add env file support to get connection string and container name");
            Console.WriteLine("Azure Blob Storage Chunk Upload Demo");
            Console.WriteLine("-----------------------------------");
            
            // Create logger
            ILogger logger = new ConsoleLogger();
            
            try
            {
                // Get storage account configuration
                var storageProvider = new AzureBlobStorageAccountProvider(logger);
                
                // Authenticate
                bool isAuthenticated = await storageProvider.AuthenticateAsync();
                if (!isAuthenticated)
                {
                    logger.LogError("Authentication failed. Please check your configuration.");
                    return;
                }
                
                // Ask for file path
                Console.Write("Enter the path to the file you want to upload: ");
                string filePath = Console.ReadLine();
                
                // Ask for chunk size (optional)
                Console.Write("Enter chunk size in MB (press Enter to use default): ");
                string chunkSizeStr = Console.ReadLine();
                
                int? chunkSize = null;
                if (!string.IsNullOrEmpty(chunkSizeStr) && int.TryParse(chunkSizeStr, out int size))
                {
                    chunkSize = size;
                }
                
                // Create progress notifier
                var progressNotifier = new ConsoleProgressNotifier();
                
                // Create file uploader
                var fileUploader = new FileUploader(
                    storageProvider.Config,
                    progressNotifier,
                    logger,
                    chunkSize);
                
                // Upload file
                bool success = await fileUploader.UploadChunksAsync(filePath);
                
                if (success)
                {
                    Console.WriteLine("\nFile uploaded successfully!");
                }
                else
                {
                    Console.WriteLine("\nFile upload failed.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred: {ex.Message}");
            }
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}