using Microsoft.VisualBasic.Logging;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.IO;
using WorshipPad.Core.Enums;
using WorshipPad.Core.Interfaces;
using WorshipPad.Core.Models;

namespace WorshipPad.Core.Services;

public class AudioPlayerService : IAudioPlayerService
{
    private IAudioOutput? output;
    private AudioFileReader? reader;

    private readonly AudioSettings _settings;
    private readonly SettingsService _settingsService;
    private readonly IAudioOutputFactory _outputFactory;
    private readonly ILogService _log;
    private MMDevice? _selectedDevice;
    
    public string? SelectedDeviceName { get; private set; }

    public float Volume
    {
        get
        {
            return reader?.Volume
                ?? (float)_settings.Volume;
        }

        set
        {
            _settings.Volume = value;

            if (reader != null)
            {
                reader.Volume = value;
            }

            _log.Info(
                $"Volume alterado para: {value:0.00}");
        }
    }

    public AudioPlayerService(
        AudioSettings audioSettings,
        SettingsService settingsService,
        IAudioOutputFactory outputFactory,
        ILogService log)
    {
        _settings = audioSettings;
        _settingsService = settingsService;
        _outputFactory = outputFactory;
        _log = log;


        // =============================
        // CARREGAR CONFIGURAÇÕES SALVAS
        // =============================

        _settings.OutputType =
            _settingsService.LoadOutputType();


        _settings.AsioDriver =
            _settingsService.LoadAsioDriver();


        _settings.AsioOutputChannel =
            _settingsService.LoadAsioOutputChannel();


        _settings.Volume =
            _settingsService.LoadVolume();


        var device =
            _settingsService.LoadOutputDevice();


        if (!string.IsNullOrWhiteSpace(device))
        {
            SetOutputDevice(device);
        }
        _log.Info(
            $"Configuração de áudio carregada. " +
            $"Tipo: {_settings.OutputType}, " +
            $"Driver ASIO: {_settings.AsioDriver ?? "nenhum"}, " +
            $"Canal ASIO: {_settings.AsioOutputChannel}, " +
            $"Volume: {_settings.Volume:0.00}");
    }


    public void PlayLoop(string filePath)
    {
        Stop();

        try
        {
            _log.Info(
                $"Iniciando pad: {Path.GetFileName(filePath)}");

            reader =
                new AudioFileReader(filePath);

            reader.Volume =
                (float)_settings.Volume;

            output =
                _outputFactory.Create(
                    _settings,
                    _selectedDevice);

            output.Init(reader);

            output.Play();

            _log.Info(
                $"Pad iniciado com sucesso: " +
                $"{Path.GetFileName(filePath)}");
        }
        catch (Exception ex)
        {
            _log.Error(
                $"Falha ao iniciar o pad: " +
                $"{Path.GetFileName(filePath)}",
                ex);

            Stop();

            throw;
        }
    }


    public void Stop()
    {
        output?.Stop();

        output?.Dispose();

        reader?.Dispose();

        output = null;
        reader = null;
    }


    public async Task FadeOut(
        int duration = 1000)
    {
        if (reader == null)
            return;


        float startVolume =
            reader.Volume;


        int steps = 20;


        for (int i = steps; i >= 0; i--)
        {
            reader.Volume =
                startVolume * i / steps;

            await Task.Delay(
                duration / steps);
        }
    }


    public async Task FadeIn(
        int duration = 1000)
    {
        if (reader == null)
            return;


        reader.Volume = 0;


        int steps = 20;


        for (int i = 0; i <= steps; i++)
        {
            reader.Volume =
                (float)i / steps;

            await Task.Delay(
                duration / steps);
        }
    }


    public void ChangeOutputDevice(string deviceName)
    {
        if (string.IsNullOrWhiteSpace(deviceName))
            return;

        try
        {
            using var enumerator =
                new MMDeviceEnumerator();

            _selectedDevice =
                enumerator
                    .EnumerateAudioEndPoints(
                        DataFlow.Render,
                        DeviceState.Active)
                    .FirstOrDefault(
                        d => d.FriendlyName == deviceName);

            if (_selectedDevice == null)
            {
                _log.Warning(
                    $"Dispositivo de áudio não encontrado: " +
                    $"{deviceName}");

                return;
            }

            SelectedDeviceName =
                deviceName;

            _settings.OutputDevice =
                deviceName;

            _log.Info(
                $"Dispositivo de saída alterado para: " +
                $"{deviceName}");

            Stop();
        }
        catch (Exception ex)
        {
            _log.Error(
                $"Erro ao alterar dispositivo de saída para " +
                $"'{deviceName}'.",
                ex);

            throw;
        }
    }


    public void SetOutputDevice(
        string deviceName)
    {
        if (string.IsNullOrWhiteSpace(deviceName))
            return;


        using var enumerator =
            new MMDeviceEnumerator();


        _selectedDevice =
            enumerator
                .EnumerateAudioEndPoints(
                    DataFlow.Render,
                    DeviceState.Active)
                .FirstOrDefault(
                    d => d.FriendlyName == deviceName);


        if (_selectedDevice == null)
            return;


        SelectedDeviceName =
            deviceName;


        _settings.OutputDevice =
            deviceName;
    }


    public void ChangeOutputType(AudioOutputType type)
    {
        _log.Info(
            $"Tipo de saída alterado para: {type}");

        _settings.OutputType = type;

        Stop();
    }


    public void ChangeOutputChannel(int channel)
    {
        if (channel <= 0)
            return;

        _settings.AsioOutputChannel =
            channel;

        _log.Info(
            $"Canal de saída ASIO alterado para: " +
            $"{channel}");

        Stop();
    }
}
