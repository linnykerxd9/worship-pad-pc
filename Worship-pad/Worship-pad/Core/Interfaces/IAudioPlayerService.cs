using WorshipPad.Core.Enums;

namespace WorshipPad.Core.Interfaces;

public interface IAudioPlayerService
{
    float Volume { get; set; }


    string? SelectedDeviceName { get; }


    void PlayLoop(
        string filePath);


    void Stop();


    void ChangeOutputChannel(
        int channel);


    void ChangeAsioSampleRate(
        int sampleRate);


    void SetOutputDevice(
        string deviceName);


    void ChangeOutputType(
        AudioOutputType value);


    void ChangeOutputDevice(
        string value);


    Task FadeIn(
        int duration = 1000);


    Task FadeOut(
        int duration = 1000);
}