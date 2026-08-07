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

        services.AddTransient<SettingsViewModel>();
        services.AddTransient<SettingsWindow>();

        // ViewModels
        services.AddTransient<MainViewModel>();


        // Views
        services.AddTransient<MainWindow>();


        Services = services.BuildServiceProvider();


        var window = Services.GetRequiredService<MainWindow>();

        window.Show();
    }
}