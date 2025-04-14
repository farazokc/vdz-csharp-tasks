classDiagram
    class VideoTranscoderApp {
        +InputVideo: string
        +OutputDirectory: string
        +TranscodingSettings: TranscodingSettings
        +TranscoderFactory: TranscoderFactory
        +ExecuteTranscoding(): void
    }

    class TranscoderFactory {
        +CreateTranscoder(outputType: string): IVideoTranscoder
    }

    class Mp4Transcoder {
        +Transcode(inputVideo: string, outputPath: string, settings: TranscodingSettings): void
    }

    class M3u8Transcoder {
        +Transcode(inputVideo: string, outputPath: string, settings: TranscodingSettings): void
    }

    class FfmpegService {
        +ExecuteFfmpegCommand(command: string): void
        +HandleProcessOutput(output: string): void
    }

    class TranscodingSettings {
        +ResolutionList: List<string>
        +Codec: string
        +Bitrate: string
    }

    VideoTranscoderApp --> TranscoderFactory : Uses
    VideoTranscoderApp --> Mp4Transcoder : Uses for MP4
    VideoTranscoderApp --> M3u8Transcoder : Uses for M3u8
    Mp4Transcoder --> FfmpegService : Uses
    M3u8Transcoder --> FfmpegService : Uses
