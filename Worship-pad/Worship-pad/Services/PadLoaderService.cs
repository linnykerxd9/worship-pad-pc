using System.IO;
using WorshipPad.Core.Enums;
using WorshipPad.Core.Helpers;
using WorshipPad.Core.Models;

namespace WorshipPad.Core.Services;

public class PadLoaderService
{
    public IReadOnlyList<PadButton> LoadPads(string folderPath)
    {
        var pads = new List<PadButton>();

        if (!Directory.Exists(folderPath))
            return pads;


        foreach (var file in Directory.GetFiles(folderPath, "*.m4a"))
        {
            var fileName = Path.GetFileNameWithoutExtension(file);

            if (Enum.TryParse<PadNote>(
                    fileName,
                    true,
                    out var note))
            {
                pads.Add(new PadButton
                {
                    Note = note,
                    DisplayName = PadHelper.ToDisplayName(note),
                    AudioPath = file
                });
            }
        }


        return pads;
    }
}