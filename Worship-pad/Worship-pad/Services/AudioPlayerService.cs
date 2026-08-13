using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.IO;
using WorshipPad.Core.Enums;
using WorshipPad.Core.Interfaces;
using WorshipPad.Core.Models;

namespace WorshipPad.Core.Services;

public class AudioPlayerService : IAudioPlayerService
{
    private IAudioOutput? output;

    private AudioFileReader? reader;

    private readonly AudioSettings _settings;

    private readonly SettingsService _settingsService;

    private readonly IAudioOutputFactory _outputFactory;

    private readonly ILogService _log;

    private MMDevice? _selectedDevice;

    private string? _currentFilePath;

    private bool _loopEnabled;

    private bool _isStopping;

    public string? SelectedDeviceName { get; private set; }

    public float Volume
    {
        get
        {
            return reader?.Volume
                ?? (float)_settings.Volume;
        }

        set
        {
            _settings.Volume = value;

            if (reader != null)
            {
                reader.Volume = value;
            }

            _log.Info(
                $"Volume alterado para: {value:0.00}");
        }
    }

    public AudioPlayerService(
        AudioSettings audioSettings,
        SettingsService settingsService,
        IAudioOutputFactory outputFactory,
        ILogService log)
    {
        _settings = audioSettings;

        _settingsService =
            settingsService;

        _outputFactory =
            outputFactory;

        _log = log;

        // =============================
        // CARREGAR CONFIGURAÇÕES
        // =============================

        _settings.OutputType =
            _settingsService.LoadOutputType();

        _settings.AsioDriver =
            _settingsService.LoadAsioDriver();

        _settings.AsioOutputChannel =
            _settingsService.LoadAsioOutputChannel();

        _settings.AsioSampleRate =
            _settingsService.LoadAsioSampleRate();

        _settings.Volume =
            _settingsService.LoadVolume();

        var device =
            _settingsService.LoadOutputDevice();

        if (!string.IsNullOrWhiteSpace(device))
        {
            SetOutputDevice(device);
        }

        _log.Info(
            $"Configuração de áudio carregada. " +
            $"Tipo: {_settings.OutputType}, " +
            $"Driver ASIO: {_settings.AsioDriver ?? "nenhum"}, " +
            $"Canal ASIO: {_settings.AsioOutputChannel}, " +
            $"Taxa ASIO: {_settings.AsioSampleRate} Hz, " +
            $"Volume: {_settings.Volume:0.00}");
    }

    // =====================================================
    // INICIAR PAD EM LOOP
    // =====================================================

    public void PlayLoop(
        string filePath)
    {
        Stop();

        try
        {
            if (!File.Exists(filePath))
            {
                _log.Warning(
                    $"Arquivo de áudio não encontrado: " +
                    $"{filePath}");

                return;
            }

            _currentFilePath =
                filePath;

            _loopEnabled = true;

            _isStopping = false;

            _log.Info(
                $"Iniciando pad em loop: " +
                $"{Path.GetFileName(filePath)}");

            CreatePlayback();

            output?.Play();

            _log.Info(
                $"Pad iniciado com sucesso: " +
                $"{Path.GetFileName(filePath)}");
        }
        catch (Exception ex)
        {
            _log.Error(
                $"Falha ao iniciar o pad: " +
                $"{Path.GetFileName(filePath)}",
                ex);

            Stop();

            throw;
        }
    }

    // =====================================================
    // CRIAR REPRODUÇÃO
    // =====================================================

    private void CreatePlayback()
    {
        if (string.IsNullOrWhiteSpace(
                _currentFilePath))
        {
            return;
        }

        // ---------------------------------------------
        // GARANTIR LIMPEZA DA REPRODUÇÃO ANTERIOR
        // ---------------------------------------------

        DisposePlayback();

        // ---------------------------------------------
        // CRIAR LEITOR
        // ---------------------------------------------

        reader =
            new AudioFileReader(
                _currentFilePath);

        _log.Info(
            $"Formato do pad: " +
            $"{reader.WaveFormat.SampleRate} Hz, " +
            $"{reader.WaveFormat.Channels} canais");

        reader.Volume =
            (float)_settings.Volume;

        // ---------------------------------------------
        // CRIAR SAÍDA
        // ---------------------------------------------

        output =
            _outputFactory.Create(
                _settings,
                _selectedDevice);

        // ---------------------------------------------
        // EVENTO DE FIM
        // ---------------------------------------------

        output.PlaybackStopped +=
            OnPlaybackStopped;

        // ---------------------------------------------
        // INICIALIZAR SAÍDA
        // ---------------------------------------------

        output.Init(
            reader);

        _log.Info(
            $"Formato de saída: " +
            $"{output.InputSampleRate} Hz → " +
            $"{output.OutputSampleRate} Hz, " +
            $"{output.OutputChannels} canal(is)");

        if (output.WasResampled)
        {
            _log.Info(
                $"Resampling aplicado: " +
                $"{output.InputSampleRate} Hz → " +
                $"{output.OutputSampleRate} Hz");
        }
        else
        {
            _log.Info(
                $"Resampling não necessário. " +
                $"Taxa mantida em " +
                $"{output.OutputSampleRate} Hz");
        }

        _log.Info(
            $"Taxa de saída efetiva do WorshipPad: " +
            $"{output.OutputSampleRate} Hz, " +
            $"{output.OutputChannels} canal(is)");
    }

    // =====================================================
    // ÁUDIO TERMINOU
    // =====================================================

    private void OnPlaybackStopped(
        object? sender,
        EventArgs e)
    {
        if (_isStopping)
        {
            return;
        }

        if (!_loopEnabled)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(
                _currentFilePath))
        {
            return;
        }

        try
        {
            _log.Info(
                $"Fim do pad detectado. " +
                $"Reiniciando: " +
                $"{Path.GetFileName(
                    _currentFilePath)}");

            RestartLoop();
        }
        catch (Exception ex)
        {
            _log.Error(
                "Erro ao reiniciar o loop do pad.",
                ex);

            Stop();
        }
    }

    // =====================================================
    // REINICIAR LOOP
    // =====================================================

    private void RestartLoop()
    {
        if (_isStopping ||
            !_loopEnabled)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(
                _currentFilePath))
        {
            return;
        }

        // Guarda o caminho porque CreatePlayback
        // recria o reader e a saída.
        var filePath =
            _currentFilePath;

        try
        {
            CreatePlayback();

            output?.Play();

            _log.Info(
                $"Loop reiniciado: " +
                $"{Path.GetFileName(filePath)}");
        }
        catch (Exception ex)
        {
            _log.Error(
                $"Falha ao reiniciar o pad: " +
                $"{Path.GetFileName(filePath)}",
                ex);

            Stop();

            throw;
        }
    }

    // =====================================================
    // LIMPAR REPRODUÇÃO
    // =====================================================

    private void DisposePlayback()
    {
        if (output != null)
        {
            output.PlaybackStopped -=
                OnPlaybackStopped;

            output.Stop();

            output.Dispose();

            output = null;
        }

        if (reader != null)
        {
            reader.Dispose();

            reader = null;
        }
    }

    // =====================================================
    // STOP
    // =====================================================

    public void Stop()
    {
        _isStopping = true;

        _loopEnabled = false;

        DisposePlayback();

        _currentFilePath = null;
    }

    // =====================================================
    // FADE OUT
    // =====================================================

    public async Task FadeOut(
        int duration = 1000)
    {
        if (reader == null)
            return;

        float startVolume =
            reader.Volume;

        int steps = 20;

        for (
            int i = steps;
            i >= 0;
            i--)
        {
            if (reader == null)
                return;

            reader.Volume =
                startVolume * i / steps;

            await Task.Delay(
                duration / steps);
        }
    }

    // =====================================================
    // FADE IN
    // =====================================================

    public async Task FadeIn(
        int duration = 1000)
    {
        if (reader == null)
            return;

        reader.Volume = 0;

        int steps = 20;

        for (
            int i = 0;
            i <= steps;
            i++)
        {
            if (reader == null)
                return;

            reader.Volume =
                (float)i / steps;

            await Task.Delay(
                duration / steps);
        }
    }

    // =====================================================
    // ALTERAR DISPOSITIVO
    // =====================================================

    public void ChangeOutputDevice(
        string deviceName)
    {
        if (string.IsNullOrWhiteSpace(
                deviceName))
        {
            return;
        }

        try
        {
            using var enumerator =
                new MMDeviceEnumerator();

            _selectedDevice =
                enumerator
                    .EnumerateAudioEndPoints(
                        DataFlow.Render,
                        DeviceState.Active)
                    .FirstOrDefault(
                        d =>
                            d.FriendlyName ==
                            deviceName);

            if (_selectedDevice == null)
            {
                _log.Warning(
                    $"Dispositivo de áudio não encontrado: " +
                    $"{deviceName}");

                return;
            }

            SelectedDeviceName =
                deviceName;

            _settings.OutputDevice =
                deviceName;

            _log.Info(
                $"Dispositivo de saída alterado para: " +
                $"{deviceName}");

            Stop();
        }
        catch (Exception ex)
        {
            _log.Error(
                $"Erro ao alterar dispositivo de saída para " +
                $"'{deviceName}'.",
                ex);

            throw;
        }
    }

    // =====================================================
    // DEFINIR DISPOSITIVO
    // =====================================================

    public void SetOutputDevice(
        string deviceName)
    {
        if (string.IsNullOrWhiteSpace(
                deviceName))
        {
            return;
        }

        using var enumerator =
            new MMDeviceEnumerator();

        _selectedDevice =
            enumerator
                .EnumerateAudioEndPoints(
                    DataFlow.Render,
                    DeviceState.Active)
                .FirstOrDefault(
                    d =>
                        d.FriendlyName ==
                        deviceName);

        if (_selectedDevice == null)
        {
            return;
        }

        SelectedDeviceName =
            deviceName;

        _settings.OutputDevice =
            deviceName;
    }

    // =====================================================
    // ALTERAR TIPO DE SAÍDA
    // =====================================================

    public void ChangeOutputType(
        AudioOutputType type)
    {
        _log.Info(
            $"Tipo de saída alterado para: " +
            $"{type}");

        _settings.OutputType =
            type;

        Stop();
    }

    // =====================================================
    // ALTERAR CANAL ASIO
    // =====================================================

    public void ChangeOutputChannel(
        int channel)
    {
        if (channel <= 0)
            return;

        _settings.AsioOutputChannel =
            channel;

        _log.Info(
            $"Canal de saída ASIO alterado para: " +
            $"{channel}");

        Stop();
    }
    public void ChangeAsioSampleRate(int sampleRate)
    {
        if (sampleRate <= 0)
            return;

        _settings.AsioSampleRate = sampleRate;

        _settingsService.SaveAsioSampleRate(sampleRate);

        _log.Info(
            $"Taxa de amostragem ASIO alterada para: {sampleRate} Hz");

        Stop();
    }
}