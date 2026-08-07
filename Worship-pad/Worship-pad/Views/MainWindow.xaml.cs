using WorshipPad.Services;
using WorshipPad.ViewModels;

namespace WorshipPad.Views;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        var bankService = new BankService(); // breakpoint aqui

        DataContext = new MainViewModel(bankService); // breakpoint aqui
    }
}