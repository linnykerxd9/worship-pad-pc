using NAudio.Wave;

namespace WorshipPad.Core.Interfaces;

public interface IAudioOutput : IDisposable
{
    int InputSampleRate { get; }

    int OutputSampleRate { get; }

    int OutputChannels { get; }

    bool WasResampled { get; }

    void Init(WaveStream stream);

    void Play();

    void Stop();

    event EventHandler? PlaybackStopped;
}