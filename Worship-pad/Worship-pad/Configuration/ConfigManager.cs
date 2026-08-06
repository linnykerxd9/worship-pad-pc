using System.IO;
using System.Text.Json;

namespace Worship_pad.Configuration;

public static class ConfigManager
{
    private const string FileName = "config.json";

    public static AppConfig Config { get; private set; } = new();

    public static void Load()
    {
        if (!File.Exists(FileName))
        {
            Save();
            return;
        }

        string json = File.ReadAllText(FileName);

        Config = JsonSerializer.Deserialize<AppConfig>(json) ?? new();
    }

    public static void Save()
    {
        var json = JsonSerializer.Serialize(
            Config,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(FileName, json);
    }
}