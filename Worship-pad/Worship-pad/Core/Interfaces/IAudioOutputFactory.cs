using NAudio.CoreAudioApi;
using WorshipPad.Core.Models;

namespace WorshipPad.Core.Interfaces;

public interface IAudioOutputFactory
{
    IAudioOutput Create(
        AudioSettings settings,
        MMDevice? device = null);
}