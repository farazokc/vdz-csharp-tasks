using Onboarding.Interfaces;
using Onboarding.Transcoders;
using System;

namespace Onboarding.Factories
{
    public class TranscoderFactory
    {
        public IVideoTranscoder CreateTranscoder(string outputType)
        {
            switch (outputType.ToLower())
            {
                case "mp4":
                    return new Mp4Transcoder();
                case "m3u8":
                    return new M3u8Transcoder();
                default:
                    throw new ArgumentException("Invalid transcoder type");
            }
        }
    }

}
