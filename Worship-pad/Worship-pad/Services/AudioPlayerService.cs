using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.Windows;

namespace WorshipPad.Core.Services;

public class AudioPlayerService
{
    private IWavePlayer? output;
    private AudioFileReader? reader;
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


        if (_selectedDevice != null)
        {
            output = new WasapiOut(
                _selectedDevice,
                AudioClientShareMode.Shared,
                true,
                100
            );
        }
        else
        {
            output = new WaveOutEvent();
        }


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