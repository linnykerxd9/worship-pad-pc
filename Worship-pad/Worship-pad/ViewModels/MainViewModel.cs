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
    
    public ObservableCollection<PadButton> Pads { get; }
    [ObservableProperty]
    private PadBank? selectedBank;
    public ObservableCollection<string> AudioDevices { get; }
    [ObservableProperty]
    private string? selectedAudioDevice;
    public ObservableCollection<PadBank> Banks { get; }
    public IRelayCommand<PadBank> SelectBankCommand { get; }
    private readonly SettingsService _settings;
    private readonly PadLoaderService _padLoaderService;
    private readonly IAudioPlayerService _audioPlayer;
    private readonly AudioDeviceService _deviceService;
    [ObservableProperty] 
    private string currentBank = "Nenhum banco";
    public IRelayCommand<PadButton> PlayPadCommand { get; }
    private PadButton? _currentPad;
    public IRelayCommand StopCommand { get; }
    public IRelayCommand<string> SelectAudioDeviceCommand { get; }
    public MainViewModel(IBankService bankService, IAudioPlayerService audioPlayer, PadLoaderService padLoaderService, SettingsService settings, AudioDeviceService deviceService)
    {
        #region DI
        _padLoaderService = padLoaderService;
        _audioPlayer = audioPlayer;
        _settings = settings;
        _deviceService = deviceService;

        #endregion

        #region Config bank
        _bankService = bankService;
        var padPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Pads",
            "Signature"
        );

        Pads = new ObservableCollection<PadButton>(
            _padLoaderService.LoadPads(padPath)
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

        StopCommand = new RelayCommand(StopPad);
        PlayPadCommand = new RelayCommand<PadButton>(PlayPad);
        #endregion

        #region Config Device
        SelectAudioDeviceCommand = new RelayCommand<string>(SelectAudioDevice);

        AudioDevices = new ObservableCollection<string>(
            _deviceService.GetOutputDevices()
        );
        _deviceService = deviceService;
        #endregion

        #region Config Settings

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



        Pads.Clear();


        foreach (var pad in _padLoaderService.LoadPads(bank.FolderPath))
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
