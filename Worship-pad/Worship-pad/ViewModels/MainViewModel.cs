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


    public ObservableCollection<PadButton> Pads { get; }

    public ObservableCollection<PadBank> Banks { get; }

    public ObservableCollection<string> AudioDevices { get; }


    [ObservableProperty]
    private PadBank? selectedBank;


    [ObservableProperty]
    private string? selectedAudioDevice;


    [ObservableProperty]
    private string currentBank = "Nenhum banco";


    public IRelayCommand<PadBank> SelectBankCommand { get; }

    public IRelayCommand<PadButton> PlayPadCommand { get; }

    public IRelayCommand StopCommand { get; }

    public IRelayCommand<string> SelectAudioDeviceCommand { get; }

    public IRelayCommand RefreshBanksCommand { get; }
    private readonly ILogService _logService;

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

        Banks =
            new ObservableCollection<PadBank>();

        AudioDevices =
            new ObservableCollection<string>(
                _deviceService.GetOutputDevices());


        // =====================================================
        // COMANDOS
        // =====================================================

        SelectBankCommand =
            new RelayCommand<PadBank>(SelectBank);

        PlayPadCommand =
            new RelayCommand<PadButton>(PlayPad);

        StopCommand =
            new RelayCommand(StopPad);

        SelectAudioDeviceCommand =
            new RelayCommand<string>(SelectAudioDevice);

        RefreshBanksCommand =
            new RelayCommand(RefreshBanks);


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


            var selectedFolderPath =
                SelectedBank?.FolderPath;


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


            Banks.Clear();


            foreach (var bank in newBanks)
            {
                Banks.Add(bank);
            }


            if (Banks.Count == 0)
            {
                SelectedBank = null;

                CurrentBank =
                    "Nenhum banco";

                Pads.Clear();

                _logService.Info(
                    "Nenhum banco disponível.");

                return;
            }


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


            bankToSelect ??= Banks[0];


            SelectBank(bankToSelect);


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

    private void SelectBank(PadBank? bank)
    {
        if (bank == null)
            return;


        foreach (var item in Banks)
        {
            item.IsSelected = false;
        }


        bank.IsSelected = true;

        SelectedBank = bank;

        CurrentBank = bank.Name;


        Pads.Clear();


        var pads =
            _padLoaderService.LoadPads(
                bank.FolderPath);


        foreach (var pad in pads)
        {
            Pads.Add(pad);
        }
    }


    // =========================================================
    // PLAY PAD
    // =========================================================

    private void PlayPad(PadButton? pad)
    {
        if (pad == null)
            return;


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

            System.Diagnostics.Debug.WriteLine(
                $"Erro ao reproduzir pad: {ex}");
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
            System.Diagnostics.Debug.WriteLine(
                $"Erro ao parar áudio: {ex}");
        }


        _currentPad = null;

        ClearPlayingPads();
    }


    // =========================================================
    // LIMPAR PAD ATUAL
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
            System.Diagnostics.Debug.WriteLine(
                $"Erro ao selecionar dispositivo: {ex}");
        }
    }


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
            System.Diagnostics.Debug.WriteLine(
                $"Erro ao alterar dispositivo: {ex}");
        }
    }


    // =========================================================
    // PAD ATUAL
    // =========================================================

    private PadButton? _currentPad;
}