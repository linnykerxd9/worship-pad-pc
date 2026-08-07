using System.Windows;
using WorshipPad.ViewModels;

namespace WorshipPad.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow(SettingsViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}