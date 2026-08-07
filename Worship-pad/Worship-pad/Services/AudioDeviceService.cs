using NAudio.CoreAudioApi;

namespace WorshipPad.Core.Services;

public class AudioDeviceService
{
    public List<string> GetOutputDevices()
    {
        var devices = new List<string>();

        using var enumerator = new MMDeviceEnumerator();


        var outputs = enumerator.EnumerateAudioEndPoints(
            DataFlow.Render,
            DeviceState.Active
        );


        foreach (var device in outputs)
        {
            devices.Add(device.FriendlyName);
        }


        return devices;
    }
}