using System.Windows;
using WorshipPad.ViewModels;

namespace WorshipPad;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainViewModel();
    }
}