using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Input;
using WorshipPad.Core.Services;
using WorshipPad.ViewModels;

namespace WorshipPad.Views;

public partial class MainWindow
{

    private readonly IServiceProvider _serviceProvider;


    public MainWindow(
        MainViewModel viewModel,
        IServiceProvider serviceProvider)
    {
        InitializeComponent();

        DataContext = viewModel;

        _serviceProvider = serviceProvider;
    }

    private void AudioDeviceChanged(
    object sender,
    SelectionChangedEventArgs e)
    {
        if (sender is ComboBox combo &&
           combo.SelectedItem is string device)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.SelectAudioDeviceCommand.Execute(device);
            }
        }
    }

    private void OpenSettings_Click(
    object sender,
    RoutedEventArgs e)
        {
            var settingsWindow = _serviceProvider
                .GetRequiredService<SettingsWindow>();

            settingsWindow.ShowDialog();
        }
}