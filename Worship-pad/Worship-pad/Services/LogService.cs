using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using WorshipPad.Core.Enums;
using WorshipPad.Core.Interfaces;
using WorshipPad.Core.Models;

namespace WorshipPad.Core.Services;

public class LogService : ILogService
{
    private readonly string _logDirectory;

    private readonly object _lock = new();


    public ObservableCollection<LogEntry> Entries { get; } =
        new();


    public LogService(AppDataService appData)
    {
        _logDirectory = appData.LogsFolder;

        Directory.CreateDirectory(
            _logDirectory);

        Refresh();
    }


    // =========================================================
    // INFO
    // =========================================================

    public void Info(string message)
    {
        Write(
            LogLevel.Info,
            message);
    }


    // =========================================================
    // WARNING
    // =========================================================

    public void Warning(string message)
    {
        Write(
            LogLevel.Warning,
            message);
    }


    // =========================================================
    // ERROR
    // =========================================================

    public void Error(string message)
    {
        Write(
            LogLevel.Error,
            message);
    }


    public void Error(
        string message,
        Exception exception)
    {
        Write(
            LogLevel.Error,
            $"{message}{Environment.NewLine}" +
            FormatException(exception));
    }


    // =========================================================
    // CRITICAL
    // =========================================================

    public void Critical(string message)
    {
        Write(
            LogLevel.Critical,
            message);
    }


    public void Critical(
        string message,
        Exception exception)
    {
        Write(
            LogLevel.Critical,
            $"{message}{Environment.NewLine}" +
            FormatException(exception));
    }


    // =========================================================
    // ATUALIZAR
    // =========================================================

    public void Refresh()
    {
        try
        {
            string filePath =
                GetLogFilePath();

            if (!File.Exists(filePath))
                return;


            var lines =
                File.ReadAllLines(
                    filePath,
                    Encoding.UTF8);


            App.Current.Dispatcher.Invoke(() =>
            {
                Entries.Clear();


                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;


                    ParseAndAddEntry(line);
                }
            });
        }
        catch (Exception exception)
        {
            // Não usamos Error() aqui para evitar
            // entrar em um ciclo caso o próprio
            // arquivo de log esteja com problema.
        }
    }


    // =========================================================
    // CAMINHO DO ARQUIVO
    // =========================================================

    public string GetLogFilePath()
    {
        return Path.Combine(
            _logDirectory,
            $"{DateTime.Now:yyyy-MM-dd}.log");
    }


    // =========================================================
    // LIMPAR
    // =========================================================

    public void Clear()
    {
        try
        {
            string filePath =
                GetLogFilePath();


            lock (_lock)
            {
                if (File.Exists(filePath))
                {
                    File.WriteAllText(
                        filePath,
                        string.Empty,
                        Encoding.UTF8);
                }
            }


            App.Current.Dispatcher.Invoke(() =>
            {
                Entries.Clear();
            });
        }
        catch (Exception exception)
        {
            Error(
                "Erro ao limpar os logs.",
                exception);
        }
    }


    // =========================================================
    // WRITE
    // =========================================================

    private void Write(
        LogLevel level,
        string message)
    {
        var entry =
            new LogEntry(
                level,
                message);


        App.Current.Dispatcher.Invoke(() =>
        {
            Entries.Add(entry);
        });


        try
        {
            string filePath =
                Path.Combine(
                    _logDirectory,
                    $"{DateTime.Now:yyyy-MM-dd}.log");


            string line =
                $"[{entry.Timestamp:HH:mm:ss}] " +
                $"[{entry.LevelText}] " +
                $"{message}";


            lock (_lock)
            {
                File.AppendAllText(
                    filePath,
                    line +
                    Environment.NewLine +
                    Environment.NewLine,
                    Encoding.UTF8);
            }
        }
        catch
        {
            // O sistema de logs nunca deve
            // derrubar o aplicativo.
        }
    }


    // =========================================================
    // PARSER
    // =========================================================

    private void ParseAndAddEntry(
        string line)
    {
        try
        {
            if (!line.StartsWith("["))
                return;


            int timeEnd =
                line.IndexOf(']');


            if (timeEnd < 0)
                return;


            int levelStart =
                line.IndexOf(
                    '[',
                    timeEnd);


            int levelEnd =
                line.IndexOf(
                    ']',
                    levelStart);


            if (levelStart < 0 ||
                levelEnd < 0)
                return;


            string timeText =
                line.Substring(
                    1,
                    timeEnd - 1);


            string levelText =
                line.Substring(
                    levelStart + 1,
                    levelEnd - levelStart - 1);


            string message =
                line.Substring(
                    levelEnd + 1)
                .Trim();


            if (!Enum.TryParse(
                    levelText,
                    true,
                    out LogLevel level))
            {
                level = LogLevel.Info;
            }


            var entry =
                new LogEntry(
                    level,
                    message);



            Entries.Add(entry);
        }
        catch
        {
            // Uma linha inválida não deve
            // impedir o carregamento das demais.
        }
    }


    // =========================================================
    // EXCEPTION FORMAT
    // =========================================================

    private static string FormatException(
        Exception exception)
    {
        var builder =
            new StringBuilder();


        builder.AppendLine(
            $"Exception: " +
            $"{exception.GetType().FullName}");


        builder.AppendLine(
            $"Message: " +
            $"{exception.Message}");


        if (exception.InnerException != null)
        {
            builder.AppendLine(
                $"InnerException: " +
                $"{exception.InnerException.Message}");
        }


        builder.AppendLine(
            $"StackTrace:" +
            $"{Environment.NewLine}" +
            exception.StackTrace);


        return builder.ToString();
    }
}