using System.IO;
using WorshipPad.Core.Interfaces;
using WorshipPad.Core.Models;

namespace WorshipPad.Services;

public class BankService : IBankService
{
    private readonly string _padsPath;


    public BankService()
    {
        _padsPath = Path.Combine(
            AppContext.BaseDirectory,
            "Pads"
        );

        EnsureFolder();
    }


    public IReadOnlyList<PadBank> GetBanks()
    {
        var folders = Directory.GetDirectories(_padsPath);

        Console.WriteLine($"Pasta de pads: {_padsPath}");
        Console.WriteLine($"Bancos encontrados: {folders.Length}");

        foreach (var folder in folders)
        {
            Console.WriteLine(folder);
        }


        return folders
            .Select(folder => new PadBank
            {
                Name = Path.GetFileName(folder),
                FolderPath = folder
            })
            .ToList();
    }


    private void EnsureFolder()
    {
        if (!Directory.Exists(_padsPath))
        {
            Directory.CreateDirectory(_padsPath);
        }
    }
}