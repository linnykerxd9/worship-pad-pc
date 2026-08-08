using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using WorshipPad.Core.Interfaces;

namespace WorshipPad.Core.Services;

public class AsioAudioOutput : IAudioOutput
{
    private readonly string _driverName;
    private readonly int _outputChannel;

    private AsioOut? _output;

    public AsioAudioOutput(
        string driverName,
        int outputChannel)
    {
        _driverName = driverName;
        _outputChannel = outputChannel;
    }

    public void Init(WaveStream stream)
    {
        var sampleProvider =
            stream.ToSampleProvider();

        var monoProvider =
            new MonoSampleProvider(
                sampleProvider);

        var waveProvider =
            new SampleToWaveProvider(
                monoProvider);

        _output =
            new AsioOut(_driverName);

        _output.ChannelOffset =
            _outputChannel - 1;

        _output.Init(waveProvider);
    }

    public void Play()
    {
        _output?.Play();
    }

    public void Stop()
    {
        _output?.Stop();
    }

    public void Dispose()
    {
        _output?.Dispose();

        _output = null;
    }
}