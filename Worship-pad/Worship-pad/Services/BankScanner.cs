using System.IO;
using WorshipPad.Models;

namespace WorshipPad.Services;

public static class BankScanner
{
    public static List<PadBank> Scan()
    {
        List<PadBank> banks = new();

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

    private static bool TryMap(string name, out PadNote note)
    {
        note = PadNote.C;

        return name switch
        {
            "c" => Set(PadNote.C),
            "csharp" => Set(PadNote.CSharp),
            "db" => Set(PadNote.CSharp),

            "d" => Set(PadNote.D),

            "dsharp" => Set(PadNote.DSharp),
            "eb" => Set(PadNote.DSharp),

            "e" => Set(PadNote.E),

            "f" => Set(PadNote.F),

            "fsharp" => Set(PadNote.FSharp),
            "gb" => Set(PadNote.FSharp),

            "g" => Set(PadNote.G),

            "gsharp" => Set(PadNote.GSharp),
            "ab" => Set(PadNote.GSharp),

            "a" => Set(PadNote.A),

            "asharp" => Set(PadNote.ASharp),
            "bb" => Set(PadNote.ASharp),

            "b" => Set(PadNote.B),

            _ => false
        };

        bool Set(PadNote n)
        {
            note = n;
            return true;
        }
    }
}