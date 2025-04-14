using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Onboarding.Services
{
    public class FfmpegService
    {
        public async Task ExecuteFfmpegCommandAsync(string command)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "C:\\Users\\Faraz.Naeem\\AppData\\Local\\Microsoft\\WinGet\\Links\\ffmpeg.exe",
                    Arguments = command,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                };

                using var process = Process.Start(processStartInfo);
                
                if (process == null)
                {
                    throw new Exception("Failed to start FFmpeg process");
                }

                string result = process.StandardOutput.ReadToEnd();

                // Wait for the process to complete
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    throw new Exception($"FFmpeg process exited with code {process.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing FFmpeg command: {ex.Message}");
            }
        }
    }
}
