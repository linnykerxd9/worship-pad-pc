using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using WorshipPad.Core.Enums;
using WorshipPad.Core.Interfaces;
using WorshipPad.Core.Models;
using WorshipPad.Core.Services;

namespace WorshipPad.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IAsioDeviceService _asioDeviceService;
    private readonly AudioDeviceService _audioDeviceService;

    public Visibility WindowsVisibility =>
    SelectedOutputType == AudioOutputType.Windows
        ? Visibility.Visible
        : Visibility.Collapsed;


    public Visibility AsioVisibility =>
        SelectedOutputType == AudioOutputType.Asio
            ? Visibility.Visible
            : Visibility.Collapsed;
    public ObservableCollection<string> AudioDevices { get; }

    public ObservableCollection<string> AsioDrivers { get; }
    private readonly IAudioPlayerService _audioPlayer;
    public ObservableCollection<AudioOutputType> OutputTypes { get; }


    [ObservableProperty]
    private AudioOutputType selectedOutputType;


    [ObservableProperty]
    private string? selectedAudioDevice;


    [ObservableProperty]
    private string? selectedAsioDriver;

    private readonly SettingsService _settings;
    private readonly AudioSettings _audioSettings;
    [ObservableProperty]
    private double volume = 0.8;

    public SettingsViewModel(
        IAsioDeviceService asioDeviceService,
        AudioDeviceService audioDeviceService,
        SettingsService settings,
        IAudioPlayerService audioPlayer,
        AudioSettings audioSettings)
    {
        _asioDeviceService = asioDeviceService;
        _audioDeviceService = audioDeviceService;
        _settings = settings;
        _audioPlayer = audioPlayer;
        _audioSettings = audioSettings;


        OutputTypes = new ObservableCollection<AudioOutputType>
        {
            AudioOutputType.Windows,
            AudioOutputType.Asio
        };


        AudioDevices = new ObservableCollection<string>(
            _audioDeviceService.GetOutputDevices()
        );


        AsioDrivers = new ObservableCollection<string>(
            _asioDeviceService.GetAsioDrivers()
        );

        SelectedOutputType = AudioOutputType.Windows;
        var savedDevice = _settings.LoadOutputDevice();


        if (savedDevice != null &&
           AudioDevices.Contains(savedDevice))
        {
            SelectedAudioDevice = savedDevice;
        }

        SelectedOutputType = _settings.LoadOutputType();

        var savedDriver = _settings.LoadAsioDriver();

        if (savedDriver != null &&
           AsioDrivers.Contains(savedDriver))
        {
            SelectedAsioDriver = savedDriver;
        }

        Volume = _settings.LoadVolume();
    }
    partial void OnVolumeChanged(double value)
    {
        _settings.SaveVolume(value);

        _audioPlayer.Volume = (float)value;
    }
    partial void OnSelectedAudioDeviceChanged(string? value)
    {
        if (value == null)
            return;


        _settings.SaveOutputDevice(value);


        _audioPlayer.ChangeOutputDevice(value);
    }
    partial void OnSelectedOutputTypeChanged(AudioOutputType value)
    {
        _settings.SaveOutputType(value);


        OnPropertyChanged(nameof(WindowsVisibility));
        OnPropertyChanged(nameof(AsioVisibility));


        _audioPlayer.ChangeOutputType(value);
    }

    partial void OnSelectedAsioDriverChanged(string? value)
    {
        if (value == null)
            return;

        _settings.SaveAsioDriver(value);
        _audioSettings.AsioDriver = value;
    }
}