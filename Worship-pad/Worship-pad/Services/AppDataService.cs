using System.IO;

namespace WorshipPad.Core.Services;

public class AppDataService
{
    public string AppDataFolder { get; }

    public string LogsFolder { get; }

    public string SettingsFile { get; }

    public AppDataService()
    {
        AppDataFolder = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "WorshipPad");

        LogsFolder = Path.Combine(
            AppDataFolder,
            "Logs");

        SettingsFile = Path.Combine(
            AppDataFolder,
            "settings.json");

        Directory.CreateDirectory(
            AppDataFolder);

        Directory.CreateDirectory(
            LogsFolder);
    }
}