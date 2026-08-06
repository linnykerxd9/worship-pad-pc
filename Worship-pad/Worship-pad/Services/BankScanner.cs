using System.Collections.ObjectModel;
using System.IO;
using Worship_pad.Core.Enums;

namespace Worship_pad.Services;

public static class BankScanner
{
    public static ObservableCollection<PadBank> Scan()
    {
        ObservableCollection<PadBank> banks = new();

        string padsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pads");

        if (!Directory.Exists(padsFolder))
            Directory.CreateDirectory(padsFolder);

        foreach (string folder in Directory.GetDirectories(padsFolder))
        {
            PadBank bank = new();

            bank.Name = Path.GetFileName(folder);

            bank.Folder = folder;

            foreach (string file in Directory.GetFiles(folder))
            {
                string name = Path.GetFileNameWithoutExtension(file).ToLower();

                if (TryMap(name, out PadNote note))
                {
                    bank.Files[note] = file;
                }
            }

            banks.Add(bank);
        }

        return banks;
    }

    private static readonly Dictionary<string, PadNote> Map = new()
    {
        ["c"] = PadNote.C,

        ["csharp"] = PadNote.CSharp,
        ["db"] = PadNote.CSharp,

        ["d"] = PadNote.D,

        ["dsharp"] = PadNote.DSharp,
        ["eb"] = PadNote.DSharp,

        ["e"] = PadNote.E,

        ["f"] = PadNote.F,

        ["fsharp"] = PadNote.FSharp,
        ["gb"] = PadNote.FSharp,

        ["g"] = PadNote.G,

        ["gsharp"] = PadNote.GSharp,
        ["ab"] = PadNote.GSharp,

        ["a"] = PadNote.A,

        ["asharp"] = PadNote.ASharp,
        ["bb"] = PadNote.ASharp,

        ["b"] = PadNote.B
    };

    private static bool TryMap(string fileName, out PadNote note)
    {
        return Map.TryGetValue(fileName, out note);
    }
}