using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Worship_pad.Models;
using Worship_pad.Services;

namespace WorshipPad.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _currentBank = "";

    [ObservableProperty]
    private string _currentPad = "--";

    [ObservableProperty]
    private double volume = 80;

    public  ObservableCollection<PadBank> Banks { get; }
    public ObservableCollection<PadButton> Buttons { get; }

    public MainViewModel()
    {
        Banks = BankScanner.Scan();

        if (Banks.Any())
            _currentBank = Banks.First().Name;
    }

    [RelayCommand]
    private void RefreshBanks()
    {
        Banks.Clear();

        foreach (var bank in BankScanner.Scan())
            Banks.Add(bank);
    }

    [RelayCommand]
    private void SelectPad(string note)
    {
        _currentPad = note;
    }
}