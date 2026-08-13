using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using WorshipPad.Core.Interfaces;

namespace WorshipPad.Core.Services;

public class AsioAudioOutput : IAudioOutput
{
    private readonly string _driverName;
    private readonly int _outputChannel;
    private readonly int _sampleRate;

    private AsioOut? _output;

    public int InputSampleRate { get; private set; }

    public int OutputSampleRate { get; private set; }

    public int OutputChannels { get; private set; }

    public bool WasResampled { get; private set; }

    public event EventHandler? PlaybackStopped;

    public AsioAudioOutput(
        string driverName,
        int outputChannel,
        int sampleRate)
    {
        _driverName = driverName;
        _outputChannel = outputChannel;
        _sampleRate = sampleRate;
    }

    public void Init(
        WaveStream stream)
    {
        InputSampleRate =
            stream.WaveFormat.SampleRate;

        var sampleProvider =
            stream.ToSampleProvider();

        // ==========================================
        // CONVERTER PARA MONO
        // ==========================================

        var monoProvider =
            new MonoSampleProvider(
                sampleProvider);

        // ==========================================
        // RESAMPLING
        // ==========================================

        ISampleProvider outputProvider =
            monoProvider;

        WasResampled = false;

        if (monoProvider.WaveFormat.SampleRate !=
            _sampleRate)
        {
            outputProvider =
                new WdlResamplingSampleProvider(
                    monoProvider,
                    _sampleRate);

            WasResampled = true;
        }

        // ==========================================
        // INFORMAÇÕES DA SAÍDA
        // ==========================================

        OutputSampleRate =
            outputProvider.WaveFormat.SampleRate;

        OutputChannels =
            outputProvider.WaveFormat.Channels;

        // ==========================================
        // SAMPLE -> WAVE
        // ==========================================

        var waveProvider =
            new SampleToWaveProvider(
                outputProvider);

        // ==========================================
        // ASIO
        // ==========================================

        _output =
            new AsioOut(
                _driverName);

        _output.ChannelOffset =
            _outputChannel - 1;

        _output.Init(
            waveProvider);

        // ==========================================
        // EVENTO DE FIM DA REPRODUÇÃO
        // ==========================================

        _output.PlaybackStopped +=
            OnPlaybackStopped;
    }

    private void OnPlaybackStopped(
        object? sender,
        StoppedEventArgs e)
    {
        PlaybackStopped?.Invoke(
            this,
            EventArgs.Empty);
    }

    public void Play()
    {
        _output?.Play();
    }

    public void Stop()
    {
        if (_output != null)
        {
            _output.Stop();
        }
    }

    public void Dispose()
    {
        if (_output != null)
        {
            _output.PlaybackStopped -=
                OnPlaybackStopped;

            _output.Dispose();

            _output = null;
        }
    }
}