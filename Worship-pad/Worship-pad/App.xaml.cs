using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WorshipPad.Core.Interfaces;
using WorshipPad.Core.Models;
using WorshipPad.Core.Services;
using WorshipPad.ViewModels;
using WorshipPad.Views;

namespace WorshipPad;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;


    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);


        var services = new ServiceCollection();

        
        // Services
        services.AddSingleton<IBankService, BankService>();
        services.AddSingleton<IAudioPlayerService, AudioPlayerService>();
        services.AddSingleton<IAudioOutputFactory, AudioOutputFactory>();
        services.AddSingleton<AudioSettings>();
        services.AddSingleton<PadLoaderService>();
        services.AddSingleton<AudioDeviceService>();
        services.AddSingleton<SettingsService>();
        services.AddSingleton<IAsioDeviceService, AsioDeviceService>();
        services.AddSingleton<ILogService, LogService>();

        services.AddTransient<LogsViewModel>();
        services.AddTransient<LogsWindow>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<SettingsWindow>();

        // ViewModels
        services.AddTransient<MainViewModel>();


        // Views
        services.AddTransient<MainWindow>();


        Services = services.BuildServiceProvider();

        var log = Services.GetRequiredService<ILogService>();

        // Exceptions da interface WPF
        DispatcherUnhandledException += (sender, args) =>
        {
            log.Error(
                "Exception não tratada na interface.",
                args.Exception);

            MessageBox.Show(
                "Ocorreu um erro no WorshipPad.\n\n" +
                "O erro foi registrado nos logs.",
                "Erro",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            // Impede o aplicativo de fechar automaticamente
            args.Handled = true;
        };

        // Exceptions não tratadas em outras threads
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            if (args.ExceptionObject is Exception exception)
            {
                log.Critical(
                    "Exception não tratada no aplicativo.",
                    exception);
            }
            else
            {
                log.Critical(
                    "Exception não tratada no aplicativo: " +
                    args.ExceptionObject);
            }
        };

        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            log.Error(
                "Exception não observada em Task.",
                args.Exception);

            args.SetObserved();
        };
        log.Info("WorshipPad iniciado.");
        var window =
            Services.GetRequiredService<MainWindow>();

        window.Show();
    }
}