using Worship_pad.Models;

public class PadBank
{
    public string Name { get; set; } = "";

    public Dictionary<PadNote, string> Notes { get; set; } = new();
}