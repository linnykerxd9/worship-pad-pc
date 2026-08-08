using WorshipPad.Core.Enums;

namespace WorshipPad.Core.Models;

public class AudioSettings
{
    public AudioOutputType OutputType { get; set; }
        = AudioOutputType.Windows;

    public string? OutputDevice { get; set; }

    public string? AsioDriver { get; set; }

    public int AsioOutputChannel { get; set; }
        = 1;

    public double Volume { get; set; }
        = 0.8;
}