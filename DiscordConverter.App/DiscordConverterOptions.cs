using CommandLine;

namespace DiscordConverter.App;

public class DiscordConverterOptions
{
    [Option("filePath", Required = false, HelpText = "Input file")]
    public string FilePath { get; set; } = string.Empty;

    [Option("openFileDialog", Required = false, HelpText = "Show open file dialog", Default = true)]
    public bool ShowFileDialog { get; set; } = true;
    
    [Option("targetSize", Required = false, HelpText = "Target file size (in MB)", Default = 10)]
    public int TargetFileSize { get; set; }
}