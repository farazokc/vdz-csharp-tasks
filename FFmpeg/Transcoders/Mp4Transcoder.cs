using Onboarding.Interfaces;
using Onboarding.Models;
using Onboarding.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Onboarding.Transcoders
{
    public class Mp4Transcoder : IVideoTranscoder
    {
        private readonly FfmpegService _ffmpegService;

        public Mp4Transcoder()
        {
            _ffmpegService = new FfmpegService();
        }

        public async Task TranscodeAsync(string inputVideo, string outputDirectory, TranscodingSettings settings)
        {
            try {
                // Remove any existing quotes and properly quote the paths
                inputVideo = inputVideo.Trim('"');
                var inputPath = $"\"{inputVideo}\"";

                foreach(var resolution in settings.ResolutionList)
                {
                    var outputPath = $"\"{Path.Combine(outputDirectory, $"output_{resolution}.mp4")}\"";

                    var command = $"-y -i {inputPath} -vf scale={resolution} -c:v libx264 {outputPath}";

                    Console.WriteLine($"Mp4 command run: {command}");

                    await _ffmpegService.ExecuteFfmpegCommandAsync(command);
                }

            }
            catch (Exception ex) {
                Console.WriteLine($"Error transcoding video: {ex.Message}");
            }
        }
    }
}
