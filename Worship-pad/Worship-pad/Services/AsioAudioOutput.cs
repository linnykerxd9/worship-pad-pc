using NAudio.Wave;
using WorshipPad.Core.Interfaces;

namespace WorshipPad.Core.Services;

public class AsioAudioOutput : IAudioOutput
{
    private readonly string _driverName;

    private AsioOut? output;


    public AsioAudioOutput(string driverName)
    {
        _driverName = driverName;
    }


    public void Init(WaveStream stream)
    {
        output = new AsioOut(_driverName);

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