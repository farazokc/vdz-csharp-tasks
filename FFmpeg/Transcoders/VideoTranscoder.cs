using Onboarding.Factories;
using Onboarding.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class VideoTranscoderApp
{
    private readonly TranscoderFactory _transcoderFactory;
    private readonly string _inputVideo;
    private readonly string _outputDirectory;
    private readonly TranscodingSettings _settings;

    public VideoTranscoderApp(string inputVideo, string outputDirectory, TranscodingSettings settings)
    {
        _inputVideo = inputVideo;
        _outputDirectory = outputDirectory;
        _settings = settings;
        _transcoderFactory = new TranscoderFactory();
    }

    public async Task ExecuteTranscoding()
    {
        var transcoders = new List<Task>();

        if (_settings.OutputFormats.Contains("mp4"))
        {
            var mp4Transcoder = _transcoderFactory.CreateTranscoder("mp4");
            transcoders.Add(mp4Transcoder.TranscodeAsync(_inputVideo, _outputDirectory, _settings));
        }

        if (_settings.OutputFormats.Contains("m3u8"))
        {
            var m3u8Transcoder = _transcoderFactory.CreateTranscoder("m3u8");
            transcoders.Add(m3u8Transcoder.TranscodeAsync(_inputVideo, _outputDirectory, _settings));
        }

        await Task.WhenAll(transcoders); // Run both transcoders concurrently
    }
}
