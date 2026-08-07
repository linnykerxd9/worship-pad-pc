using CommunityToolkit.Mvvm.ComponentModel;
using WorshipPad.Core.Enums;

namespace WorshipPad.Core.Models;

public partial class PadButton : ObservableObject
{
    public PadNote Note { get; set; }

    public string DisplayName { get; set; } = "";

    public string AudioPath { get; set; } = "";

    [ObservableProperty]
    private bool isPlaying;
}