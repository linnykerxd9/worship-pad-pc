using NAudio.Wave;

namespace WorshipPad.Core.Interfaces;

public interface IAudioOutput
{
    void Init(WaveStream stream);

    void Play();

    void Stop();

    void Dispose();
}