using System.Windows;
using WorshipPad.ViewModels;

namespace WorshipPad.Views;

public partial class LogsWindow : Window
{
    public LogsWindow(
        LogsViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}