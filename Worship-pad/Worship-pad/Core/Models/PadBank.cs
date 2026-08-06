using WorshipPad.Core.Enums;

namespace WorshipPad.Core.Models;

public class PadBank
{
    public string Name { get; set; } = "";

    public string Path { get; set; } = "";

    public Dictionary<PadNote, string> Notes { get; } = [];
}