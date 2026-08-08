using WorshipPad.Core.Enums;

namespace WorshipPad.Core.Models;

public class LogEntry
{
    public DateTime Timestamp { get; }
    public LogLevel Level { get; }
    public string Message { get; }

    public LogEntry(
        LogLevel level,
        string message)
    {
        Timestamp = DateTime.Now;
        Level = level;
        Message = message;
    }

    public string Time =>
        Timestamp.ToString("HH:mm:ss");

    public string LevelText =>
        Level.ToString().ToUpper();
}