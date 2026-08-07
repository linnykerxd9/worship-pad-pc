using System.IO;
using WorshipPad.Core.Interfaces;
using WorshipPad.Core.Models;

namespace WorshipPad.Core.Services;

public class BankService : IBankService
{
    private readonly string _padsFolder;


    public BankService()
    {
        _padsFolder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Pads"
        );
    }


    public IReadOnlyList<PadBank> GetBanks()
    {
        if (!Directory.Exists(_padsFolder))
            return new List<PadBank>();


        return Directory
            .GetDirectories(_padsFolder)
            .Select(folder => new PadBank
            {
                Name = Path.GetFileName(folder),
                FolderPath = folder
            })
            .ToList();
    }
}