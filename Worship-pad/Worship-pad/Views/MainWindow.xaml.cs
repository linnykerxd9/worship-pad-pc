using WorshipPad.ViewModels;

namespace WorshipPad;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainViewModel();
    }
}