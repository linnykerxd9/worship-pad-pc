using WorshipPad.Core.Enums;

namespace WorshipPad.Core.Models;

public class AudioSettings
{
    public AudioOutputType OutputType { get; set; } = AudioOutputType.Windows;

    public string? DeviceName { get; set; }

    public string? AsioDriver { get; set; }
}