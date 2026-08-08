using System.IO;
using WorshipPad.Core.Interfaces;
using WorshipPad.Core.Models;

namespace WorshipPad.Core.Services;

public class BankService : IBankService
{
    private readonly string _padsFolder;


    public BankService()
    {
        _padsFolder =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Pads");
    }


    public IReadOnlyList<PadBank> GetBanks()
    {
        if (!Directory.Exists(_padsFolder))
            return Array.Empty<PadBank>();


        return Directory
            .EnumerateDirectories(_padsFolder)
            .Select(folder => new PadBank
            {
                Name = Path.GetFileName(folder),

                FolderPath = folder,

                PadCount =
                    Directory
                        .EnumerateFiles(folder)
                        .Count(file =>
                            file.EndsWith(
                                ".wav",
                                StringComparison.OrdinalIgnoreCase)
                            ||
                            file.EndsWith(
                                ".mp3",
                                StringComparison.OrdinalIgnoreCase)
                            ||
                            file.EndsWith(
                                ".m4a",
                                StringComparison.OrdinalIgnoreCase))
            })
            .OrderBy(bank =>
                bank.Name.Equals(
                    "Worship App Maior",
                    StringComparison.OrdinalIgnoreCase)
                    ? 0
                    :
                bank.Name.Equals(
                    "Worship App Menor",
                    StringComparison.OrdinalIgnoreCase)
                    ? 1
                    :
                    2)
            .ThenBy(bank => bank.Name)
            .ToList();
    }


    private static bool IsAudioFile(
        string file)
    {
        var extension =
            Path.GetExtension(file);


        return
            string.Equals(
                extension,
                ".mp3",
                StringComparison.OrdinalIgnoreCase)
            ||
            string.Equals(
                extension,
                ".m4a",
                StringComparison.OrdinalIgnoreCase)
            ||
            string.Equals(
                extension,
                ".wav",
                StringComparison.OrdinalIgnoreCase);
    }
}