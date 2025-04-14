using Onboarding.Interfaces;
using System;
using System.IO;
using System.Text;

namespace Onboarding.Helpers
{
    public class FileLogger : ILogger
    {
        private readonly string _logFilePath;
        
        public FileLogger(string logFilePath = null)
        {
            _logFilePath = logFilePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "upload.log");
            
            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath));
        }
        
        public void Log(string message)
        {
            WriteToFile($"[INFO] {DateTime.Now}: {message}");
        }

        public void LogError(string message)
        {
            WriteToFile($"[ERROR] {DateTime.Now}: {message}");
        }
        
        private void WriteToFile(string content)
        {
            try
            {
                File.AppendAllText(_logFilePath, content + Environment.NewLine, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                // Fall back to console if file writing fails
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
                Console.WriteLine(content);
            }
        }
    }
}