namespace Worship_pad.Models;

public class PadBank
{
    public string Name { get; set; } = "";

    public string Folder { get; set; } = "";

    public Dictionary<PadNote, string> Files { get; set; } = new();
}