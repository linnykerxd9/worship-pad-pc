using NAudio.CoreAudioApi;
using NAudio.Wave;
using WorshipPad.Core.Interfaces;

namespace WorshipPad.Core.Services;

public class WindowsAudioOutput : IAudioOutput
{
    private IWavePlayer? output;

    private readonly MMDevice? device;


    public WindowsAudioOutput(MMDevice? selectedDevice)
    {
        device = selectedDevice;
    }


    public void Init(WaveStream stream)
    {
        if (device != null)
        {
            output = new WasapiOut(
                device,
                AudioClientShareMode.Shared,
                true,
                100);
        }
        else
        {
            output = new WaveOutEvent();
        }


        output.Init(stream);
    }


    public void Play()
    {
        output?.Play();
    }


    public void Stop()
    {
        output?.Stop();
    }


    public void Dispose()
    {
        output?.Dispose();
        output = null;
    }
}
