using WorshipPad.Core.Enums;

namespace WorshipPad.Core.Models;

public class PadButton
{
    public PadNote Note { get; set; }

    public string DisplayName { get; set; } = "";

    public string AudioPath { get; set; } = "";

}