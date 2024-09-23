using System;
using System.Linq;
#if WINDOWS
using System.Windows.Forms;
#endif
using CommandLine;

namespace DiscordConverter.App;

internal static class Program
{
#if WINDOWS
    [STAThread]
#endif
    private static void Main(string[] args)
    {
        var result = Parser.Default.ParseArguments<DiscordConverterOptions>(args);
        if (result.Errors.Any())
        {
            return;
        }
        
        var options = result.Value;
        
#if WINDOWS
        if (options.ShowFileDialog)
        {
            options.FilePath = GetFilePathFromDialog();
        }
#endif

        new DiscordConverter(options).Run();
    }
    
#if WINDOWS
    private static string GetFilePathFromDialog()
    {
        using var dialog = new OpenFileDialog();
        dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        dialog.Title = "Select a video file";
        dialog.Filter = "Video Files|*.mp4;*.avi;*.mov;*.mkv;*.flv;*.wmv;*.webm";
        dialog.RestoreDirectory = true;
        
        return dialog.ShowDialog() != DialogResult.OK ? string.Empty : dialog.FileName;
    }
#endif
}