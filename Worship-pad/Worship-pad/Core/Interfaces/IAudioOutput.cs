using NAudio.Wave;

namespace WorshipPad.Core.Interfaces;

public interface IAudioOutput : IDisposable
{
    void Init(WaveStream stream);

    void Play();

    void Stop();
}