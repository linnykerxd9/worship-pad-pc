using System.IO;
using System.Text.Json;
using WorshipPad.Core.Enums;

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



    private AppSettings LoadSettings()
    {
        if (!File.Exists(_filePath))
            return new AppSettings();


        var json = File.ReadAllText(_filePath);


        return JsonSerializer.Deserialize<AppSettings>(json)
               ?? new AppSettings();
    }



    private void SaveSettings(AppSettings settings)
    {
        var json = JsonSerializer.Serialize(
            settings,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });


        File.WriteAllText(
            _filePath,
            json
        );
    }



    public void SaveOutputDevice(string device)
    {
        var settings = LoadSettings();

        settings.OutputDevice = device;

        SaveSettings(settings);
    }



    public string? LoadOutputDevice()
    {
        return LoadSettings().OutputDevice;
    }



    public void SaveOutputType(AudioOutputType type)
    {
        var settings = LoadSettings();

        settings.OutputType = type;

        SaveSettings(settings);
    }



    public AudioOutputType LoadOutputType()
    {
        return LoadSettings().OutputType;
    }



    public void SaveAsioDriver(string driver)
    {
        var settings = LoadSettings();

        settings.AsioDriver = driver;

        SaveSettings(settings);
    }



    public string? LoadAsioDriver()
    {
        return LoadSettings().AsioDriver;
    }



    public void SaveVolume(double volume)
    {
        var settings = LoadSettings();

        settings.Volume = volume;

        SaveSettings(settings);
    }



    public double LoadVolume()
    {
        return LoadSettings().Volume;
    }
}