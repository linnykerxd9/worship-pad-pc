using NAudio.CoreAudioApi;
using NAudio.Wave;
using WorshipPad.Core.Interfaces;

namespace WorshipPad.Core.Services;

public class WindowsAudioOutput : IAudioOutput
{
    private IWavePlayer? output;

    private readonly MMDevice? device;

    public int InputSampleRate { get; private set; }

    public int OutputSampleRate { get; private set; }

    public int OutputChannels { get; private set; }

    public bool WasResampled => false;

    public event EventHandler? PlaybackStopped;

    public WindowsAudioOutput(
        MMDevice? selectedDevice)
    {
        device = selectedDevice;
    }

    public void Init(
        WaveStream stream)
    {
        InputSampleRate =
            stream.WaveFormat.SampleRate;

        OutputSampleRate =
            stream.WaveFormat.SampleRate;

        OutputChannels =
            stream.WaveFormat.Channels;

        // ==========================================
        // CRIAR SAÍDA
        // ==========================================

        if (device != null)
        {
            output =
                new WasapiOut(
                    device,
                    AudioClientShareMode.Shared,
                    true,
                    100);
        }
        else
        {
            output =
                new WaveOutEvent();
        }

        // ==========================================
        // EVENTO DE FIM DA REPRODUÇÃO
        // ==========================================

        output.PlaybackStopped +=
            OnPlaybackStopped;

        // ==========================================
        // INICIALIZAR ÁUDIO
        // ==========================================

        output.Init(
            stream);
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
        output?.Play();
    }

    public void Stop()
    {
        if (output != null)
        {
            output.Stop();
        }
    }

    public void Dispose()
    {
        if (output != null)
        {
            output.PlaybackStopped -=
                OnPlaybackStopped;

            output.Dispose();

            output = null;
        }
    }
}