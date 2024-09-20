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
        CreateRegistryFiles();
        
        var result = Parser.Default.ParseArguments<DiscordConverterOptions>(args);
        if (result.Errors.Any())
        {
            return;
        }
        
        var options = result.Value;
        if (string.IsNullOrEmpty(options.FilePath))
        {
            options.FilePath = GetFilePathFromDialog();
        }

        new DiscordConverter(options).RunAsync().Wait();
    }

    private static void CreateRegistryFiles()
    {
        var templateFileNames = new[] {"RegisterDiscordConverterTemplate.reg", "UnregisterDiscordConverterTemplate.reg"};

        if (File.Exists(templateFileNames[0].Replace("Template.", ".")))
        {
            return;
        }

        var exePath = Environment.ProcessPath;
            
        foreach (var tfn in templateFileNames)
        {
            var template = File.ReadAllText(tfn);
            
            var modified = template.Replace("<EXE_PATH>", exePath);
            
            File.WriteAllText(tfn.Replace("Template.", "."), modified);
        }
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