using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using WorshipPad.Core.Enums;
using WorshipPad.Core.Helpers;
using WorshipPad.Core.Models;

namespace WorshipPad.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public ObservableCollection<PadButton> Pads { get; }


    [ObservableProperty]
    private string currentBank = "Signature";


    [ObservableProperty]
    private int volume = 80;



    public MainViewModel()
    {
        Pads = new ObservableCollection<PadButton>(
            PadHelper.CreateButtons()
        );
    }
}