using System;
using System.IO;
using System.Threading.Tasks;
using FFMpegCore;
using FFMpegCore.Enums;

namespace DiscordConverter.App;

public class DiscordConverter(DiscordConverterOptions options)
{
    public void Run()
    {
        try
        {
            ProcessAsync().Wait();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private async Task ProcessAsync()
    {
        var filePath = options.FilePath;
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }
        
        var file = File.OpenRead(filePath);
        if (file.Length < options.TargetFileSize * 1024 * 1024)
        {
            throw new ApplicationException("File is smaller than target file size");
        }
        
        var mediaInfo = await FFProbe.AnalyseAsync(options.FilePath);
        
        var targetBitrate = (options.TargetFileSize * 1024 * 8) / mediaInfo.Duration.TotalSeconds; // MB -> kBit / seconds
        targetBitrate *= .9; // Random factor
        
        const int audioBitrate = 128;
        var videoBitrate = (int)Math.Floor(targetBitrate - audioBitrate);

        var targetFilePath = Path.Combine(Path.GetDirectoryName(options.FilePath)!, $"{Path.GetFileNameWithoutExtension(options.FilePath)}_converted.mp4");

        Console.WriteLine("Pass 1:");

        const string tmpFilePath = "tmp.mp4";
        
        await CreateConversionPass(options.FilePath, tmpFilePath, videoBitrate, audioBitrate, 1)
            .NotifyOnProgress(OnFFmpegProgress)
            .ProcessAsynchronously();
        
        File.Delete(tmpFilePath);
        
        Console.WriteLine("Pass 2:");
        
        await CreateConversionPass(options.FilePath, targetFilePath, videoBitrate, audioBitrate, 2)
            .NotifyOnProgress(OnFFmpegProgress)
            .ProcessAsynchronously();
        
        Console.WriteLine("Done");
    }

    private static FFMpegArgumentProcessor CreateConversionPass(string inputFilePath, string targetFilePath, int videoBitrate, int audioBitrate, int pass)
    {
        return FFMpegArguments.FromFileInput(inputFilePath, true, args => args
                .WithHardwareAcceleration())
            .OutputToFile(targetFilePath, true, args => args
                .WithVideoCodec(VideoCodec.LibX264)
                .WithVideoBitrate(videoBitrate)
                .WithAudioCodec(AudioCodec.Aac)
                .WithAudioBitrate(audioBitrate)
                .WithCustomArgument($"-pass {pass}"));
    }

    private static void OnFFmpegProgress(TimeSpan progress)
    {
        Console.WriteLine($"{progress.TotalSeconds}");
    }
}