using System.IO;
using WorshipPad.Core.Interfaces;
using WorshipPad.Core.Models;

namespace WorshipPad.Core.Services;

public class BankService : IBankService
{
    private readonly string _padsFolder;
    private readonly PadLoaderService _padLoaderService;


    public BankService(
        PadLoaderService padLoaderService)
    {
        _padLoaderService = padLoaderService;

        _padsFolder =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Pads");
    }


    public IReadOnlyList<PadBank> GetBanks()
    {
        if (!Directory.Exists(_padsFolder))
            return Array.Empty<PadBank>();


        var banks =
            new List<PadBank>();


        foreach (var folder in
                 Directory.EnumerateDirectories(
                     _padsFolder))
        {
            try
            {
                // =================================================
                // CARREGA OS PADS NOVAMENTE DO DISCO
                // =================================================

                var pads =
                    _padLoaderService.LoadPads(
                        folder);


                banks.Add(
                    new PadBank
                    {
                        Name =
                            Path.GetFileName(folder),

                        FolderPath =
                            folder,

                        PadCount =
                            pads.Count
                    });
            }
            catch
            {
                // Se houver algum problema ao ler uma pasta,
                // ainda mantemos o banco na lista.

                banks.Add(
                    new PadBank
                    {
                        Name =
                            Path.GetFileName(folder),

                        FolderPath =
                            folder,

                        PadCount = 0
                    });
            }
        }


        // =====================================================
        // ORDEM DOS BANCOS
        // =====================================================

        return banks
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

            .ThenBy(
                bank => bank.Name)

            .ToList();
    }
}