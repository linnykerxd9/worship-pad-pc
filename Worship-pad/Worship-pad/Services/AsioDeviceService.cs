using NAudio.Wave;
using WorshipPad.Core.Interfaces;

namespace WorshipPad.Core.Services;

public class AsioDeviceService : IAsioDeviceService
{
    public IReadOnlyList<string> GetAsioDrivers()
    {
        return AsioOut
            .GetDriverNames()
            .ToList();
    }
}