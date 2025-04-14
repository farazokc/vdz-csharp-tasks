using System.Collections.Generic;

namespace Onboarding.Models
{
    public class TranscodingSettings
    {
        public List<string> ResolutionList { get; set; } = new List<string>();
        public string Codec { get; set; }
        public string Bitrate { get; set; }
        public List<string> OutputFormats { get; set; } = new List<string>();
    }
}
