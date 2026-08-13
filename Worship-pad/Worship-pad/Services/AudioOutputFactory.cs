using NAudio.CoreAudioApi;
using WorshipPad.Core.Enums;
using WorshipPad.Core.Interfaces;
using WorshipPad.Core.Models;

namespace WorshipPad.Core.Services;

public class AudioOutputFactory : IAudioOutputFactory
{
    public IAudioOutput Create(
        AudioSettings settings,
        MMDevice? device = null)
    {
        return settings.OutputType switch
        {
            AudioOutputType.Windows =>
                new WindowsAudioOutput(
                    device),

            AudioOutputType.Asio =>
                new AsioAudioOutput(
                    settings.AsioDriver
                        ?? throw new InvalidOperationException(
                            "Driver ASIO não selecionado."),
                    settings.AsioOutputChannel,
                    settings.AsioSampleRate),

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(settings.OutputType))
        };
    }
}