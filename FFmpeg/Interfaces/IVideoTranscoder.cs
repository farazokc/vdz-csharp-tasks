using Onboarding.Models;
using System.Threading.Tasks;

namespace Onboarding.Interfaces
{
    public interface IVideoTranscoder
    {
        Task TranscodeAsync(string inputVideo, string outputDirectory, TranscodingSettings settings);
    }
}
