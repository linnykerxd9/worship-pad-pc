using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using WorshipPad.Core.Helpers;
using WorshipPad.Core.Interfaces;
using WorshipPad.Core.Models;
using WorshipPad.Core.Services;
using CommunityToolkit.Mvvm.Input;

namespace WorshipPad.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IBankService _bankService;
    [ObservableProperty]
    private double volume = 0.8;

    public ObservableCollection<PadButton> Pads { get; }
    [ObservableProperty]
    private PadBank? selectedBank;
    public ObservableCollection<string> AudioDevices { get; }
    [ObservableProperty]
    private string? selectedAudioDevice;
    public ObservableCollection<PadBank> Banks { get; }
    public IRelayCommand<PadBank> SelectBankCommand { get; }
    private readonly SettingsService _settings;
    private readonly AudioPlayerService _audioPlayer;
    [ObservableProperty]
    private string currentBank = "Nenhum banco";
    public IRelayCommand<PadButton> PlayPadCommand { get; }
    private PadButton? _currentPad;
    public IRelayCommand StopCommand { get; }
    public IRelayCommand<string> SelectAudioDeviceCommand { get; }
    public MainViewModel(IBankService bankService)
    {
        #region Config bank
        _bankService = bankService;
        var padPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Pads",
            "Signature"
        );

        var loader = new PadLoaderService();


        Pads = new ObservableCollection<PadButton>(
            loader.LoadPads(padPath)
        );


        Banks = new ObservableCollection<PadBank>(
            _bankService.GetBanks()
        );
        SelectBankCommand = new RelayCommand<PadBank>(SelectBank);

        if (Banks.Count > 0)
        {
            Banks[0].IsSelected = true;
            CurrentBank = Banks[0].Name;

            SelectBank(Banks[0]);
        }
        #endregion

        #region Config Audio

        _audioPlayer = new AudioPlayerService();
        StopCommand = new RelayCommand(StopPad);
        PlayPadCommand = new RelayCommand<PadButton>(PlayPad);
        #endregion

        #region Config Device
        var deviceService = new AudioDeviceService();
        SelectAudioDeviceCommand = new RelayCommand<string>(SelectAudioDevice);

        AudioDevices = new ObservableCollection<string>(
            deviceService.GetOutputDevices()
        );
        #endregion

        #region Config Settings
        _settings = new SettingsService();

        var savedDevice = _settings.LoadOutputDevice();


        if (savedDevice != null &&
           AudioDevices.Contains(savedDevice))
        {
            SelectedAudioDevice = savedDevice;

            _audioPlayer.SetOutputDevice(savedDevice);
        }
        #endregion
    }

    private void SelectBank(PadBank? bank)
    {
        if (bank == null)
            return;

        foreach (var item in Banks)
        {
            item.IsSelected = false;
        }

        bank.IsSelected = true;
        CurrentBank = bank.Name;

        var loader = new PadLoaderService();


        Pads.Clear();


        foreach (var pad in loader.LoadPads(bank.FolderPath))
        {
            Pads.Add(pad);
        }
    }
    private void PlayPad(PadButton pad)
    {
        ClearPlayingPads();


        pad.IsPlaying = true;


        _audioPlayer.PlayLoop(pad.AudioPath);
    }

    private void StopPad()
    {
        _audioPlayer.Stop();

        _currentPad = null;
        ClearPlayingPads();
    }
    private void ClearPlayingPads()
    {
        foreach (var pad in Pads)
        {
            pad.IsPlaying = false;
        }
    }
    partial void OnVolumeChanged(double value)
    {
        _audioPlayer.Volume = (float)value;
    }
    private void SelectAudioDevice(string? device)
    {
        if (device == null)
            return;


        _audioPlayer.SetOutputDevice(device);
    }

    partial void OnSelectedAudioDeviceChanged(string? value)
    {
        if (value == null)
            return;


        _audioPlayer.SetOutputDevice(value);

        _settings.SaveOutputDevice(value);
    }
}
