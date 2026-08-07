using System.IO;
using System.Text.Json;

namespace WorshipPad.Core.Services;

public class SettingsService
{
    private readonly string _filePath;


    public SettingsService()
    {
        _filePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "settings.json"
        );
    }



    public void SaveOutputDevice(string device)
    {
        var settings = new AppSettings
        {
            OutputDevice = device
        };


        var json = JsonSerializer.Serialize(settings);


        File.WriteAllText(
            _filePath,
            json
        );
    }



    public string? LoadOutputDevice()
    {
        if (!File.Exists(_filePath))
            return null;


        var json = File.ReadAllText(_filePath);


        var settings =
            JsonSerializer.Deserialize<AppSettings>(json);


        return settings?.OutputDevice;
    }
}



public class AppSettings
{
    public string? OutputDevice { get; set; }
}