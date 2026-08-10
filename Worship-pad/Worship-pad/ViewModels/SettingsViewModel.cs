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
    private readonly ILogService _log;
    private readonly IAudioPlayerService _audioPlayer;
    private readonly SettingsService _settings;
    private readonly AudioSettings _audioSettings;


    // =========================================================
    // COLEÇÕES
    // =========================================================

    public ObservableCollection<int> AsioSampleRates { get; }

    public ObservableCollection<int> AsioOutputChannels { get; }

    public ObservableCollection<string> AudioDevices { get; }

    public ObservableCollection<string> AsioDrivers { get; }

    public ObservableCollection<AudioOutputType> OutputTypes { get; }


    // =========================================================
    // VISIBILIDADE
    // =========================================================

    public Visibility WindowsVisibility =>
        SelectedOutputType == AudioOutputType.Windows
            ? Visibility.Visible
            : Visibility.Collapsed;


    public Visibility AsioVisibility =>
        SelectedOutputType == AudioOutputType.Asio
            ? Visibility.Visible
            : Visibility.Collapsed;


    // =========================================================
    // PROPRIEDADES
    // =========================================================

    [ObservableProperty]
    private AudioOutputType selectedOutputType;


    [ObservableProperty]
    private string? selectedAudioDevice;


    [ObservableProperty]
    private int selectedAsioSampleRate;


    [ObservableProperty]
    private string? selectedAsioDriver;


    [ObservableProperty]
    private int selectedAsioOutputChannel;


    [ObservableProperty]
    private double volume = 0.8;


    // =========================================================
    // CONSTRUTOR
    // =========================================================

    public SettingsViewModel(
        IAsioDeviceService asioDeviceService,
        AudioDeviceService audioDeviceService,
        SettingsService settings,
        IAudioPlayerService audioPlayer,
        AudioSettings audioSettings,
        ILogService log)
    {
        _asioDeviceService = asioDeviceService;
        _audioDeviceService = audioDeviceService;
        _settings = settings;
        _audioPlayer = audioPlayer;
        _audioSettings = audioSettings;
        _log = log;


        // =====================================================
        // COLEÇÕES
        // =====================================================

        AsioOutputChannels =
            new ObservableCollection<int>();


        AsioSampleRates =
            new ObservableCollection<int>
            {
                44100,
                48000,
                88200,
                96000
            };


        OutputTypes =
            new ObservableCollection<AudioOutputType>
            {
                AudioOutputType.Windows,
                AudioOutputType.Asio
            };


        AudioDevices =
            new ObservableCollection<string>(
                _audioDeviceService.GetOutputDevices());


        AsioDrivers =
            new ObservableCollection<string>(
                _asioDeviceService.GetAsioDrivers());


        // =====================================================
        // CARREGAR CONFIGURAÇÕES SALVAS
        // =====================================================

        selectedOutputType =
            _settings.LoadOutputType();


        var savedDevice =
            _settings.LoadOutputDevice();


        if (savedDevice != null &&
            AudioDevices.Contains(savedDevice))
        {
            selectedAudioDevice =
                savedDevice;
        }


        var savedDriver =
            _settings.LoadAsioDriver();


        if (savedDriver != null &&
            AsioDrivers.Contains(savedDriver))
        {
            selectedAsioDriver =
                savedDriver;
        }


        // =====================================================
        // TAXA DE AMOSTRAGEM
        // =====================================================

        var savedSampleRate =
            _settings.LoadAsioSampleRate();


        // Se não existir uma taxa válida salva,
        // usamos 44100 Hz como padrão.
        if (!AsioSampleRates.Contains(savedSampleRate))
        {
            savedSampleRate = 44100;
        }


        selectedAsioSampleRate =
            savedSampleRate;


        _audioSettings.AsioSampleRate =
            savedSampleRate;


        // =====================================================
        // VOLUME
        // =====================================================

        volume =
            _settings.LoadVolume();


        // =====================================================
        // CANAL ASIO
        // =====================================================

        selectedAsioOutputChannel =
            _settings.LoadAsioOutputChannel();


        // =====================================================
        // CARREGAR CANAIS ASIO
        // =====================================================

        if (!string.IsNullOrWhiteSpace(
            selectedAsioDriver))
        {
            LoadAsioOutputChannels(
                selectedAsioDriver);
        }


        // =====================================================
        // ATUALIZAR VISIBILIDADE
        // =====================================================

        OnPropertyChanged(
            nameof(WindowsVisibility));


        OnPropertyChanged(
            nameof(AsioVisibility));


        // =====================================================
        // LOG
        // =====================================================

        _log.Info(
            $"Tela de configurações carregada. " +
            $"Tipo: {SelectedOutputType}, " +
            $"Dispositivo: {SelectedAudioDevice ?? "nenhum"}, " +
            $"Driver ASIO: {SelectedAsioDriver ?? "nenhum"}, " +
            $"Taxa ASIO: {SelectedAsioSampleRate} Hz, " +
            $"Canal ASIO: {SelectedAsioOutputChannel}, " +
            $"Volume: {Volume:0.00}");
    }


    // =========================================================
    // VOLUME
    // =========================================================

    partial void OnVolumeChanged(double value)
    {
        _settings.SaveVolume(value);

        _audioPlayer.Volume =
            (float)value;
    }


    // =========================================================
    // DISPOSITIVO WINDOWS
    // =========================================================

    partial void OnSelectedAudioDeviceChanged(
        string? value)
    {
        if (value == null)
            return;


        _settings.SaveOutputDevice(value);


        _log.Info(
            $"Dispositivo Windows selecionado: {value}");


        _audioPlayer.ChangeOutputDevice(value);
    }


    // =========================================================
    // TIPO DE SAÍDA
    // =========================================================

    partial void OnSelectedOutputTypeChanged(
        AudioOutputType value)
    {
        _settings.SaveOutputType(value);


        _log.Info(
            $"Tipo de saída alterado para: {value}");


        OnPropertyChanged(
            nameof(WindowsVisibility));


        OnPropertyChanged(
            nameof(AsioVisibility));


        _audioPlayer.ChangeOutputType(value);
    }


    // =========================================================
    // DRIVER ASIO
    // =========================================================

    partial void OnSelectedAsioDriverChanged(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;


        _settings.SaveAsioDriver(value);


        _audioSettings.AsioDriver =
            value;


        _log.Info(
            $"Driver ASIO selecionado: {value}");


        LoadAsioOutputChannels(value);


        if (!AsioOutputChannels.Contains(
                SelectedAsioOutputChannel))
        {
            SelectedAsioOutputChannel =
                AsioOutputChannels.FirstOrDefault();
        }
    }


    // =========================================================
    // TAXA DE AMOSTRAGEM ASIO
    // =========================================================

    partial void OnSelectedAsioSampleRateChanged(
        int value)
    {
        if (value <= 0)
            return;


        _settings.SaveAsioSampleRate(value);


        _audioSettings.AsioSampleRate =
            value;


        _log.Info(
            $"Taxa de amostragem ASIO alterada para: " +
            $"{value} Hz");


        _audioPlayer.ChangeAsioSampleRate(
            value);
    }


    // =========================================================
    // CANAL ASIO
    // =========================================================

    partial void OnSelectedAsioOutputChannelChanged(
        int value)
    {
        if (value <= 0)
            return;


        _settings.SaveAsioOutputChannel(value);


        _audioSettings.AsioOutputChannel =
            value;


        _log.Info(
            $"Canal de saída ASIO selecionado: {value}");


        _audioPlayer.ChangeOutputChannel(value);
    }


    // =========================================================
    // CARREGAR CANAIS ASIO
    // =========================================================

    private void LoadAsioOutputChannels(
        string driverName)
    {
        AsioOutputChannels.Clear();


        try
        {
            var count =
                _asioDeviceService
                    .GetOutputChannelCount(
                        driverName);


            for (int i = 1; i <= count; i++)
            {
                AsioOutputChannels.Add(i);
            }
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(
                $"Não foi possível consultar os canais " +
                $"do driver ASIO '{driverName}'. " +
                $"O dispositivo pode não estar conectado.");


            _log.Error(
                $"Detalhes da falha ao consultar o driver ASIO " +
                $"'{driverName}'.",
                ex);
        }
    }
}