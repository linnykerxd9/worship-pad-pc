using NAudio.Wave;

namespace WorshipPad.Core.Services;

public class AsioDeviceService : IAsioDeviceService
{
    public IReadOnlyList<string> GetAsioDrivers()
    {
        return AsioOut.GetDriverNames();
    }

    public int GetOutputChannelCount(string driverName)
    {
        using var asio = new AsioOut(driverName);

        return asio.DriverOutputChannelCount;
    }

    public IReadOnlyList<string> GetOutputChannels(string driverName)
    {
        using var asio = new AsioOut(driverName);

        var channels = new List<string>();

        for (int i = 0; i < asio.DriverOutputChannelCount; i++)
        {
            var name = asio.AsioOutputChannelName(i);

            if (string.IsNullOrWhiteSpace(name))
                name = $"Canal {i + 1}";

            channels.Add($"{i + 1} - {name}");
        }

        return channels;
    }
}