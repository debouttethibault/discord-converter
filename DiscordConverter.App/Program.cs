using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CommandLine;
using FFMpegCore.Exceptions;

namespace DiscordConverter.App;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        var result = Parser.Default.ParseArguments<DiscordConverterOptions>(args);
        if (result.Errors.Any())
        {
            return;
        }
        
        var options = result.Value;
        
        if (options.ShowFileDialog)
        {
            options.FilePath = GetFilePathFromDialog();
        }

        new DiscordConverter(options).RunAsync().Wait();
    }
    
    private static string GetFilePathFromDialog()
    {
        using var dialog = new OpenFileDialog();
        dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        dialog.Title = "Select a video file";
        dialog.Filter = "Video Files|*.mp4;*.avi;*.mov;*.mkv;*.flv;*.wmv;*.webm";
        dialog.RestoreDirectory = true;
        dialog.CheckFileExists = true;
        
        if (dialog.ShowDialog() != DialogResult.OK)
        {
            throw new ApplicationException("No video file selected");
        }

        return dialog.FileName;
    }
}