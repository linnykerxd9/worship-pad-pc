public interface IAsioDeviceService
{
    IReadOnlyList<string> GetAsioDrivers();

    int GetOutputChannelCount(string driverName);
}