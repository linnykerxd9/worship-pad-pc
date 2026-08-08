using System.Collections.ObjectModel;
using WorshipPad.Core.Models;

namespace WorshipPad.Core.Interfaces;

public interface ILogService
{
    ObservableCollection<LogEntry> Entries { get; }

    void Info(string message);

    void Warning(string message);

    void Error(string message);

    void Error(
        string message,
        Exception exception);

    void Critical(string message);

    void Critical(
        string message,
        Exception exception);

    // Operações da tela de logs
    void Refresh();

    string? GetLogFilePath();

    void Clear();
}