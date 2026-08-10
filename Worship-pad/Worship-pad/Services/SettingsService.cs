using System.IO;
using System.Text.Json;
using WorshipPad.Core.Enums;

namespace WorshipPad.Core.Services;

public class SettingsService
{
    private readonly string _filePath;


    public SettingsService(
        AppDataService appData)
    {
        _filePath =
            appData.SettingsFile;
    }


    // =========================================================
    // CARREGAR CONFIGURAÇÕES
    // =========================================================

    private AppSettings LoadSettings()
    {
        if (!File.Exists(_filePath))
            return new AppSettings();


        var json =
            File.ReadAllText(_filePath);


        return JsonSerializer.Deserialize<AppSettings>(
                   json)
               ?? new AppSettings();
    }


    // =========================================================
    // SALVAR CONFIGURAÇÕES
    // =========================================================

    private void SaveSettings(
        AppSettings settings)
    {
        var json =
            JsonSerializer.Serialize(
                settings,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });


        File.WriteAllText(
            _filePath,
            json);
    }


    // =========================================================
    // DISPOSITIVO WINDOWS
    // =========================================================

    public void SaveOutputDevice(
        string device)
    {
        var settings =
            LoadSettings();


        settings.OutputDevice =
            device;


        SaveSettings(settings);
    }


    public string? LoadOutputDevice()
    {
        return LoadSettings()
            .OutputDevice;
    }


    // =========================================================
    // TIPO DE SAÍDA
    // =========================================================

    public void SaveOutputType(
        AudioOutputType type)
    {
        var settings =
            LoadSettings();


        settings.OutputType =
            type;


        SaveSettings(settings);
    }


    public AudioOutputType LoadOutputType()
    {
        return LoadSettings()
            .OutputType;
    }


    // =========================================================
    // DRIVER ASIO
    // =========================================================

    public void SaveAsioDriver(
        string driver)
    {
        var settings =
            LoadSettings();


        settings.AsioDriver =
            driver;


        SaveSettings(settings);
    }


    public string? LoadAsioDriver()
    {
        return LoadSettings()
            .AsioDriver;
    }


    // =========================================================
    // VOLUME
    // =========================================================

    public void SaveVolume(
        double volume)
    {
        var settings =
            LoadSettings();


        settings.Volume =
            volume;


        SaveSettings(settings);
    }


    public double LoadVolume()
    {
        return LoadSettings()
            .Volume;
    }


    // =========================================================
    // TAXA DE AMOSTRAGEM ASIO
    // =========================================================

    public void SaveAsioSampleRate(
        int sampleRate)
    {
        var settings =
            LoadSettings();


        settings.AsioSampleRate =
            sampleRate;


        SaveSettings(settings);
    }


    public int LoadAsioSampleRate()
    {
        var sampleRate =
            LoadSettings()
                .AsioSampleRate;


        // Compatibilidade com configurações antigas
        // que ainda não possuíam essa propriedade.
        if (sampleRate <= 0)
            return 44100;


        return sampleRate;
    }


    // =========================================================
    // CANAL ASIO
    // =========================================================

    public void SaveAsioOutputChannel(
        int channel)
    {
        var settings =
            LoadSettings();


        settings.AsioOutputChannel =
            channel;


        SaveSettings(settings);
    }


    public int LoadAsioOutputChannel()
    {
        return LoadSettings()
            .AsioOutputChannel;
    }
}