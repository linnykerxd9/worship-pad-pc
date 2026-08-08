using WorshipPad.Core.Enums;

public class AppSettings
{
    public string? OutputDevice { get; set; }

    public AudioOutputType OutputType { get; set; } = AudioOutputType.Windows;

    public string? AsioDriver { get; set; }
    public int AsioOutputChannel { get; set; } = 1;
    public double Volume { get; set; } = 0.8;
}