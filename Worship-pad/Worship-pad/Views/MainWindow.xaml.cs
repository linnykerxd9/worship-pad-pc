using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WorshipPad.Core.Services;
using WorshipPad.ViewModels;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WorshipPad.Views;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        var bankService = new BankService(); // breakpoint aqui

        DataContext = new MainViewModel(bankService); // breakpoint aqui
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
        var window = new SettingsWindow();

        window.DataContext = DataContext;

        window.ShowDialog();
    }
}