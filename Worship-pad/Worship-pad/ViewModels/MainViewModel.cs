using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using WorshipPad.Core.Helpers;
using WorshipPad.Core.Interfaces;
using WorshipPad.Core.Models;
using WorshipPad.Core.Services;


namespace WorshipPad.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IBankService _bankService;


    public ObservableCollection<PadButton> Pads { get; }


    public ObservableCollection<PadBank> Banks { get; }



    [ObservableProperty]
    private string currentBank = "Nenhum banco";



    public MainViewModel(IBankService bankService)
    {
        _bankService = bankService;

        var padPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Pads",
            "Signature"
        );

        var loader = new PadLoaderService();


        Pads = new ObservableCollection<PadButton>(
            loader.LoadPads(padPath)
        );


        Banks = new ObservableCollection<PadBank>(
            _bankService.GetBanks()
        );


        if (Banks.Count > 0)
        {
            CurrentBank = Banks[0].Name;
        }
    }
}