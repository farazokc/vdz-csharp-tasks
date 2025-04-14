using Onboarding.Models;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Read and deserialize the settings from the fixed path PROJECT/Configuration/AppSettings.json
        var settings = ReadSettingsFromJson("Configuration/AppSettings.json");

        // Ask the user for the input video path and output directory at runtime
        Console.WriteLine("Please enter the path to the input video file:");
        string inputVideo = Console.ReadLine();

        Console.WriteLine("Please enter the output directory path:");
        string outputDirectory = Console.ReadLine();

        // Ensure the input path and output directory are provided, otherwise, set defaults
        inputVideo = string.IsNullOrEmpty(inputVideo) ? "path_to_input_video.mp4" : inputVideo;
        outputDirectory = string.IsNullOrEmpty(outputDirectory) ? "output_directory" : outputDirectory;

        Console.WriteLine($"inputVideo: {inputVideo}");
        Console.WriteLine($"outputDirectory: {outputDirectory}");

        // Initialize the VideoTranscoderApp with the loaded settings
        var app = new VideoTranscoderApp(inputVideo, outputDirectory, settings);

        // Execute the transcoding process
        await app.ExecuteTranscoding();
    }

    // Method to read and deserialize settings from AppSettings.json
    public static TranscodingSettings ReadSettingsFromJson(string filePath)
    {
        // Check if the file exists before reading
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Error: The configuration file at {filePath} does not exist.");
            return null;  // Handle the error accordingly
        }

        // Read the JSON content from the file
        var jsonContent = File.ReadAllText(filePath);

        // Deserialize the JSON content into TranscodingSettings object
        var settings = JsonSerializer.Deserialize<TranscodingSettings>(jsonContent);

        // Return the populated settings
        return settings;
    }
}
