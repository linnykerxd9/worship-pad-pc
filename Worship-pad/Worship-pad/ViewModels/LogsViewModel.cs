using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using WorshipPad.Core.Interfaces;
using WorshipPad.Core.Models;

namespace WorshipPad.ViewModels;

public partial class LogsViewModel : ObservableObject
{
    private readonly ILogService _log;

    public ObservableCollection<LogEntry> Entries =>
        _log.Entries;


    public LogsViewModel(ILogService log)
    {
        _log = log;
    }


    // =========================================================
    // ATUALIZAR
    // =========================================================

    [RelayCommand]
    private void Refresh()
    {
        try
        {
            _log.Refresh();

            OnPropertyChanged(nameof(Entries));
        }
        catch (Exception ex)
        {
            _log.Error(
                $"Erro ao atualizar os logs: {ex.Message}");
        }
    }


    // =========================================================
    // ABRIR ARQUIVO
    // =========================================================

    [RelayCommand]
    private void OpenFile()
    {
        try
        {
            string? path = _log.GetLogFilePath();

            if (string.IsNullOrWhiteSpace(path))
            {
                _log.Warning(
                    "Não foi possível localizar o arquivo de log.");

                return;
            }

            if (!File.Exists(path))
            {
                _log.Warning(
                    "O arquivo de log ainda não existe.");

                return;
            }

            Process.Start(
                new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });
        }
        catch (Exception ex)
        {
            _log.Error(
                $"Erro ao abrir o arquivo de log: {ex.Message}");
        }
    }


    // =========================================================
    // LIMPAR
    // =========================================================

    [RelayCommand]
    private void Clear()
    {
        try
        {
            _log.Clear();

            OnPropertyChanged(nameof(Entries));
        }
        catch (Exception ex)
        {
            _log.Error(
                $"Erro ao limpar os logs: {ex.Message}");
        }
    }
}