using System.IO;
using WorshipPad.Core.Enums;
using WorshipPad.Core.Helpers;
using WorshipPad.Core.Models;

namespace WorshipPad.Core.Services;

public class PadLoaderService
{
    public IReadOnlyList<PadButton> LoadPads(
        string folderPath)
    {
        var pads =
            new List<PadButton>();


        if (!Directory.Exists(folderPath))
            return pads;


        var files =
            Directory.EnumerateFiles(folderPath)
            .Where(file =>
            {
                var extension =
                    Path.GetExtension(file);

                return
                    string.Equals(
                        extension,
                        ".m4a",
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    string.Equals(
                        extension,
                        ".mp3",
                        StringComparison.OrdinalIgnoreCase);
            });


        foreach (var file in files)
        {
            var fileName =
                Path.GetFileNameWithoutExtension(file);


            if (!TryParseNote(
                    fileName,
                    out var note))
            {
                continue;
            }


            pads.Add(
                new PadButton
                {
                    Note = note,

                    DisplayName =
                        PadHelper.ToDisplayName(note),

                    AudioPath = file
                });
        }


        return pads;
    }


    private static bool TryParseNote(
        string fileName,
        out PadNote note)
    {
        note = default;


        // Remove o sufixo _minor.
        //
        // a_minor
        //      ↓
        // a
        //
        // a_sharp_minor
        //      ↓
        // a_sharp

        string normalized =
            fileName;


        if (normalized.EndsWith(
                "_minor",
                StringComparison.OrdinalIgnoreCase))
        {
            normalized =
                normalized[..^6];
        }


        // Converte:
        //
        // a_sharp
        //      ↓
        // ASharp

        normalized =
            normalized.Replace(
                "_sharp",
                "Sharp",
                StringComparison.OrdinalIgnoreCase);


        // Também aceita:
        //
        // a-sharp

        normalized =
            normalized.Replace(
                "-sharp",
                "Sharp",
                StringComparison.OrdinalIgnoreCase);


        return Enum.TryParse(
            normalized,
            true,
            out note);
    }
}