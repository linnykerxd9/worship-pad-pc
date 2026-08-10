using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
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


    public ObservableCollection<PadButton> Pads { get; }

    public ObservableCollection<PadBank> Banks { get; }

    public ObservableCollection<string> AudioDevices { get; }

    public ObservableCollection<PadButton> FilteredPads { get; }


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


    public IRelayCommand<PadBank> SelectBankCommand { get; }

    public IRelayCommand<PadButton> PlayPadCommand { get; }

    public IRelayCommand StopCommand { get; }

    public IRelayCommand<string> SelectAudioDeviceCommand { get; }

    public IRelayCommand RefreshBanksCommand { get; }


    private PadButton? _currentPad;


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
            // GUARDA O BANCO ATUAL
            // =================================================

            var selectedFolderPath =
                SelectedBank?.FolderPath;


            // =================================================
            // LER BANCOS NOVAMENTE DO DISCO
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


                _logService.Info(
                    "Nenhum banco disponível.");


                return;
            }


            // =================================================
            // ENCONTRAR O BANCO ANTERIOR
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


            // Se o banco anterior não existe mais,
            // seleciona o primeiro banco.

            bankToSelect ??=
                Banks[0];


            // =================================================
            // RECARREGAR O BANCO ATUAL
            // =================================================

            SelectBank(
                bankToSelect);


            _logService.Info(
                $"Banco selecionado: " +
                $"{bankToSelect.Name}");


            _logService.Info(
                $"Pads carregados: " +
                $"{Pads.Count}");


            _logService.Info(
                "Atualização dos bancos concluída.");
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
        // MARCAR BANCO SELECIONADO
        // =====================================================

        foreach (var item in Banks)
        {
            item.IsSelected = false;
        }


        bank.IsSelected = true;


        SelectedBank =
            bank;


        CurrentBank =
            bank.Name;


        SearchText =
            "";


        // =====================================================
        // LIMPAR LISTAS ATUAIS
        // =====================================================

        Pads.Clear();

        FilteredPads.Clear();


        // =====================================================
        // RECARREGAR PADS DIRETAMENTE DO DISCO
        // =====================================================

        var pads =
            _padLoaderService.LoadPads(
                bank.FolderPath);


        // =====================================================
        // ADICIONAR PADS NOVAMENTE
        // =====================================================

        foreach (var pad in pads)
        {
            Pads.Add(pad);

            FilteredPads.Add(pad);
        }


        // =====================================================
        // IDENTIFICAR TIPO DO BANCO
        // =====================================================

        IsNoteBank =
            pads.Any(
                pad => pad.Note.HasValue);


        // =====================================================
        // ATUALIZAR CONTAGEM DO BANCO
        // =====================================================

        bank.PadCount =
            pads.Count;
    }


    // =========================================================
    // PESQUISA
    // =========================================================

    partial void OnSearchTextChanged(
        string value)
    {
        UpdateFilteredPads();
    }


    private void UpdateFilteredPads()
    {
        FilteredPads.Clear();


        var search =
            SearchText?.Trim();


        IEnumerable<PadButton> filtered =
            Pads;


        if (!string.IsNullOrWhiteSpace(
            search))
        {
            filtered =
                Pads.Where(
                    pad =>
                        pad.DisplayName.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase));
        }


        foreach (var pad in filtered)
        {
            FilteredPads.Add(pad);
        }
    }


    // =========================================================
    // PLAY PAD
    // =========================================================

    private void PlayPad(
        PadButton? pad)
    {
        if (pad == null)
            return;


        ClearPlayingPads();


        pad.IsPlaying =
            true;


        _currentPad =
            pad;


        try
        {
            _audioPlayer.PlayLoop(
                pad.AudioPath);
        }
        catch (Exception ex)
        {
            pad.IsPlaying =
                false;


            _currentPad =
                null;


            _logService.Error(
                $"Erro ao reproduzir pad: " +
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


        _currentPad =
            null;


        ClearPlayingPads();
    }


    // =========================================================
    // LIMPAR PAD ATUAL
    // =========================================================

    private void ClearPlayingPads()
    {
        foreach (var pad in Pads)
        {
            pad.IsPlaying =
                false;
        }
    }


    // =========================================================
    // DISPOSITIVO DE ÁUDIO
    // =========================================================

    private void SelectAudioDevice(
        string? device)
    {
        if (string.IsNullOrWhiteSpace(
            device))
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
        if (string.IsNullOrWhiteSpace(
            value))
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
