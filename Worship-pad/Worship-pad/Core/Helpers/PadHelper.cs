using WorshipPad.Core.Enums;
using WorshipPad.Core.Models;

namespace WorshipPad.Core.Helpers;

public static class PadHelper
{
    private static readonly Dictionary<PadNote, string> DisplayNames = new()
    {
        { PadNote.C, "C" },
        { PadNote.CSharp, "C#" },
        { PadNote.D, "D" },
        { PadNote.DSharp, "D#" },
        { PadNote.E, "E" },
        { PadNote.F, "F" },
        { PadNote.FSharp, "F#" },
        { PadNote.G, "G" },
        { PadNote.GSharp, "G#" },
        { PadNote.A, "A" },
        { PadNote.ASharp, "A#" },
        { PadNote.B, "B" }
    };

    public static string ToDisplayName(PadNote note)
    {
        return DisplayNames[note];
    }
}