using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WorshipPad.ViewModels;

namespace WorshipPad.Views;

public partial class MainWindow : Window
{
    private readonly IServiceProvider _services;

    public MainWindow(
        MainViewModel viewModel,
        IServiceProvider services)
    {
        InitializeComponent();

        DataContext = viewModel;

        _services = services;
    }

    private void OpenSettings_Click(
        object sender,
        RoutedEventArgs e)
    {
        var settingsWindow =
            _services.GetRequiredService<SettingsWindow>();

        settingsWindow.Owner = this;

        settingsWindow.ShowDialog();
    }

    private void OpenLogs_Click(
        object sender,
        RoutedEventArgs e)
    {
        var logsWindow =
            _services.GetRequiredService<LogsWindow>();

        logsWindow.Owner = this;

        logsWindow.ShowDialog();
    }
}