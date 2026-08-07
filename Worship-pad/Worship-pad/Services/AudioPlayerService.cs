using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Runtime;
using System.Windows;
using WorshipPad.Core.Enums;
using WorshipPad.Core.Interfaces;
using WorshipPad.Core.Models;

namespace WorshipPad.Core.Services;

public class AudioPlayerService : IAudioPlayerService
{
    private IAudioOutput? output;
    private AudioFileReader? reader;
    private AudioSettings? _settings;
    private readonly SettingsService _settingsService;
    private readonly IAudioOutputFactory _outputFactory;
    private string? _currentFile;
    public AudioPlayerService(
    AudioSettings audioSettings,
    SettingsService settingsService,
    IAudioOutputFactory outputFactory)
    {
        _settings = audioSettings;
        _settingsService = settingsService;
        _outputFactory = outputFactory;


        _settings.OutputType =
            _settingsService.LoadOutputType();


        _settings.AsioDriver =
            _settingsService.LoadAsioDriver();


        var device =
            _settingsService.LoadOutputDevice();


        if (device != null)
        {
            SetOutputDevice(device);
        }
    }

    private MMDevice? _selectedDevice;
    public string? SelectedDeviceName { get; private set; }
    public float Volume
    {
        get
        {
            return reader?.Volume ?? 1f;
        }

        set
        {
            if (reader != null)
            {
                reader.Volume = value;
            }
        }
    }
    public void PlayLoop(string filePath)
    {
        Stop();


        reader = new AudioFileReader(filePath);


        reader.Volume = (float)_settingsService.LoadVolume();


        output = _outputFactory.Create(
            _settings,
            _selectedDevice
        );


        output.Init(reader);

        output.Play();
    }



    public void Stop()
    {
        output?.Stop();

        output?.Dispose();
        reader?.Dispose();

        output = null;
        reader = null;
    }

    public async Task FadeOut(int duration = 1000)
    {
        if (reader == null)
            return;


        float startVolume = reader.Volume;


        int steps = 20;

        for (int i = steps; i >= 0; i--)
        {
            reader.Volume = startVolume * i / steps;

            await Task.Delay(duration / steps);
        }
    }
    public void ChangeOutputDevice(string deviceName)
    {
        bool wasPlaying = output != null;

        var oldPosition = reader?.CurrentTime ?? TimeSpan.Zero;


        SetOutputDevice(deviceName);


        if (reader == null || !wasPlaying)
            return;


        output?.Stop();
        output?.Dispose();


        output = _outputFactory.Create(
            _settings,
            _selectedDevice
        );


        output.Init(reader);

        reader.CurrentTime = oldPosition;

        output.Play();
    }
    public void ChangeOutputType(AudioOutputType type)
    {
        bool wasPlaying = output != null;


        if (reader == null || !wasPlaying)
        {
            _settings.OutputType = type;
            return;
        }


        var currentPosition = reader.CurrentTime;


        output?.Stop();
        output?.Dispose();


        _settings.OutputType = type;


        output = _outputFactory.Create(
            _settings,
            _selectedDevice
        );


        output.Init(reader);


        reader.CurrentTime = currentPosition;


        output.Play();
    }
    public async Task FadeIn(int duration = 1000)
    {
        if (reader == null)
            return;


        reader.Volume = 0;


        int steps = 20;


        for (int i = 0; i <= steps; i++)
        {
            reader.Volume = (float)i / steps;

            await Task.Delay(duration / steps);
        }
    }
    public void SetOutputDevice(string deviceName)
    {
        SelectedDeviceName = deviceName;

        using var enumerator = new MMDeviceEnumerator();

        _selectedDevice = enumerator
            .EnumerateAudioEndPoints(
                DataFlow.Render,
                DeviceState.Active
            )
            .FirstOrDefault(d => d.FriendlyName == deviceName);
    }

    private int GetDeviceNumber(MMDevice device)
    {
        using var enumerator = new MMDeviceEnumerator();

        var devices = enumerator
            .EnumerateAudioEndPoints(
                DataFlow.Render,
                DeviceState.Active
            );


        for (int i = 0; i < devices.Count; i++)
        {
            if (devices[i].ID == device.ID)
                return i;
        }


        return 0;
    }
}