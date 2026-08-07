namespace WorshipPad.Core.Interfaces;

public interface IAsioDeviceService
{
    IReadOnlyList<string> GetAsioDrivers();
}