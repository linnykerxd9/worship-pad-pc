using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using WorshipPad.Core.Interfaces;
using WorshipPad.Core.Models;
using WorshipPad.Core.Services;

namespace WorshipPad.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IBankService _bankService;
    private readonly PadLoaderService _padLoaderService;
    private readonly IAudioPlayerService _audioPlayer;
    private readonly SettingsService _settings;
    private readonly AudioDeviceService _deviceService;
    private readonly ILogService _logService;


    // =========================================================
    // COLEÇÕES
    // =========================================================

    public ObservableCollection<PadButton> Pads { get; }

    public ObservableCollection<PadButton> FilteredPads { get; }

    public ObservableCollection<PadBank> Banks { get; }

    public ObservableCollection<string> AudioDevices { get; }


    // =========================================================
    // ESTADO
    // =========================================================

    [ObservableProperty]
    private bool isNoteBank;


    [ObservableProperty]
    private PadBank? selectedBank;


    [ObservableProperty]
    private string? selectedAudioDevice;


    [ObservableProperty]
    private string searchText = "";


    [ObservableProperty]
    private string currentBank = "Nenhum banco";


    private PadButton? _currentPad;


    // =========================================================
    // COMANDOS
    // =========================================================

    public IRelayCommand<PadBank> SelectBankCommand { get; }

    public IRelayCommand<PadButton> PlayPadCommand { get; }

    public IRelayCommand StopCommand { get; }

    public IRelayCommand<string> SelectAudioDeviceCommand { get; }

    public IRelayCommand RefreshBanksCommand { get; }


    // =========================================================
    // CONSTRUTOR
    // =========================================================

    public MainViewModel(
        IBankService bankService,
        IAudioPlayerService audioPlayer,
        PadLoaderService padLoaderService,
        SettingsService settings,
        AudioDeviceService deviceService,
        ILogService logService)
    {
        _bankService = bankService;
        _audioPlayer = audioPlayer;
        _padLoaderService = padLoaderService;
        _settings = settings;
        _deviceService = deviceService;
        _logService = logService;


        // =====================================================
        // COLEÇÕES
        // =====================================================

        Pads =
            new ObservableCollection<PadButton>();


        FilteredPads =
            new ObservableCollection<PadButton>();


        Banks =
            new ObservableCollection<PadBank>();


        AudioDevices =
            new ObservableCollection<string>(
                _deviceService.GetOutputDevices());


        // =====================================================
        // COMANDOS
        // =====================================================

        SelectBankCommand =
            new RelayCommand<PadBank>(
                SelectBank);


        PlayPadCommand =
            new RelayCommand<PadButton>(
                PlayPad);


        StopCommand =
            new RelayCommand(
                StopPad);


        SelectAudioDeviceCommand =
            new RelayCommand<string>(
                SelectAudioDevice);


        RefreshBanksCommand =
            new RelayCommand(
                RefreshBanks);


        // =====================================================
        // CARREGAMENTO INICIAL
        // =====================================================

        RefreshBanks();
    }


    // =========================================================
    // BANCOS
    // =========================================================

    private void RefreshBanks()
    {
        try
        {
            _logService.Info(
                "Atualização dos bancos iniciada.");


            // =================================================
            // GUARDAR BANCO ATUAL
            // =================================================

            var selectedFolderPath =
                SelectedBank?.FolderPath;


            // =================================================
            // LER BANCOS DO DISCO
            // =================================================

            var newBanks =
                _bankService.GetBanks();


            _logService.Info(
                $"Foram encontrados {newBanks.Count} bancos no disco.");


            foreach (var bank in newBanks)
            {
                _logService.Info(
                    $"Banco encontrado: {bank.Name} " +
                    $"({bank.PadCount} pads)");
            }


            // =================================================
            // ATUALIZAR COLEÇÃO DE BANCOS
            // =================================================

            Banks.Clear();


            foreach (var bank in newBanks)
            {
                Banks.Add(bank);
            }


            // =================================================
            // NENHUM BANCO
            // =================================================

            if (Banks.Count == 0)
            {
                SelectedBank = null;

                CurrentBank =
                    "Nenhum banco";

                IsNoteBank = false;

                Pads.Clear();

                FilteredPads.Clear();

                return;
            }


            // =================================================
            // ENCONTRAR BANCO ANTERIOR
            // =================================================

            PadBank? bankToSelect = null;


            if (!string.IsNullOrWhiteSpace(
                    selectedFolderPath))
            {
                bankToSelect =
                    Banks.FirstOrDefault(
                        bank =>
                            string.Equals(
                                bank.FolderPath,
                                selectedFolderPath,
                                StringComparison.OrdinalIgnoreCase));
            }


            // Se o banco anterior não existir,
            // seleciona o primeiro.

            bankToSelect ??=
                Banks[0];


            // =================================================
            // SELECIONAR BANCO
            // =================================================

            SelectBank(
                bankToSelect);


            _logService.Info(
                $"Banco selecionado: {bankToSelect.Name}");


            _logService.Info(
                $"Pads carregados: {Pads.Count}");
        }
        catch (Exception ex)
        {
            _logService.Error(
                "Erro ao atualizar os bancos.",
                ex);
        }
    }


    // =========================================================
    // SELECIONAR BANCO
    // =========================================================

    private void SelectBank(
        PadBank? bank)
    {
        if (bank == null)
            return;


        // =====================================================
        // DESMARCAR BANCOS ANTERIORES
        // =====================================================

        foreach (var item in Banks)
        {
            item.IsSelected = false;
        }


        // =====================================================
        // SELECIONAR BANCO
        // =====================================================

        bank.IsSelected = true;

        SelectedBank = bank;

        CurrentBank = bank.Name;


        // =====================================================
        // LIMPAR PESQUISA
        // =====================================================

        SearchText = "";


        // =====================================================
        // LIMPAR ÁUDIOS ANTERIORES
        // =====================================================

        Pads.Clear();

        FilteredPads.Clear();


        // =====================================================
        // CARREGAR NOVOS ÁUDIOS
        // =====================================================

        var pads =
            _padLoaderService.LoadPads(
                bank.FolderPath);


        foreach (var pad in pads)
        {
            Pads.Add(pad);
        }


        // =====================================================
        // IDENTIFICAR TIPO DO BANCO
        // =====================================================

        IsNoteBank =
            pads.Any(
                pad => pad.Note.HasValue);


        // =====================================================
        // ATUALIZAR LISTA FILTRADA
        // =====================================================

        UpdateFilteredPads();
    }


    // =========================================================
    // FILTRO / PESQUISA
    // =========================================================
    // =========================================================
    // NORMALIZAR TEXTO PARA PESQUISA
    // =========================================================

    private static string NormalizeSearchText(
        string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;


        var normalized =
            text.Normalize(
                NormalizationForm.FormD);


        var builder =
            new StringBuilder();


        foreach (var character in normalized)
        {
            var category =
                CharUnicodeInfo.GetUnicodeCategory(
                    character);


            // Ignora acentos
            if (category ==
                UnicodeCategory.NonSpacingMark)
            {
                continue;
            }


            builder.Append(character);
        }


        return
            builder
                .ToString()
                .Normalize(
                    NormalizationForm.FormC)
                .ToLowerInvariant()
                .Trim();
    }


    // =========================================================
    // FILTRAR ÁUDIOS
    // =========================================================

    private void UpdateFilteredPads()
    {
        FilteredPads.Clear();


        var search =
            NormalizeSearchText(
                SearchText);


        // -----------------------------------------------------
        // Sem pesquisa: mostra tudo
        // -----------------------------------------------------

        if (string.IsNullOrWhiteSpace(search))
        {
            foreach (var pad in Pads)
            {
                FilteredPads.Add(pad);
            }

            return;
        }


        // -----------------------------------------------------
        // Pesquisa ignorando:
        //
        // - maiúsculas/minúsculas
        // - acentos
        // -----------------------------------------------------

        foreach (var pad in Pads)
        {
            var displayName =
                NormalizeSearchText(
                    pad.DisplayName);


            if (displayName.Contains(search))
            {
                FilteredPads.Add(pad);
            }
        }
    }
    // =========================================================
    // QUANDO O TEXTO DA PESQUISA MUDA
    // =========================================================

    partial void OnSearchTextChanged(
        string value)
    {
        UpdateFilteredPads();
    }


    // =========================================================
    // PLAY PAD / ÁUDIO
    // =========================================================

    private void PlayPad(
        PadButton? pad)
    {
        if (pad == null)
            return;


        // =====================================================
        // LIMPAR ÁUDIO ANTERIOR
        // =====================================================

        ClearPlayingPads();


        pad.IsPlaying = true;

        _currentPad = pad;


        try
        {
            _audioPlayer.PlayLoop(
                pad.AudioPath);
        }
        catch (Exception ex)
        {
            pad.IsPlaying = false;

            _currentPad = null;


            _logService.Error(
                $"Erro ao reproduzir áudio: " +
                $"{Path.GetFileName(pad.AudioPath)}",
                ex);
        }
    }


    // =========================================================
    // STOP
    // =========================================================

    private void StopPad()
    {
        try
        {
            _audioPlayer.Stop();
        }
        catch (Exception ex)
        {
            _logService.Error(
                "Erro ao parar áudio.",
                ex);
        }


        _currentPad = null;

        ClearPlayingPads();
    }


    // =========================================================
    // LIMPAR INDICADOR DE REPRODUÇÃO
    // =========================================================

    private void ClearPlayingPads()
    {
        foreach (var pad in Pads)
        {
            pad.IsPlaying = false;
        }
    }


    // =========================================================
    // DISPOSITIVO DE ÁUDIO
    // =========================================================

    private void SelectAudioDevice(
        string? device)
    {
        if (string.IsNullOrWhiteSpace(device))
            return;


        try
        {
            _audioPlayer.SetOutputDevice(
                device);
        }
        catch (Exception ex)
        {
            _logService.Error(
                $"Erro ao selecionar dispositivo: {device}",
                ex);
        }
    }


    // =========================================================
    // DISPOSITIVO DE ÁUDIO ALTERADO
    // =========================================================

    partial void OnSelectedAudioDeviceChanged(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;


        try
        {
            _audioPlayer.SetOutputDevice(
                value);


            _settings.SaveOutputDevice(
                value);
        }
        catch (Exception ex)
        {
            _logService.Error(
                $"Erro ao alterar dispositivo de áudio: {value}",
                ex);
        }
    }
}
