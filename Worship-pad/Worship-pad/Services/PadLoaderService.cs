using System.Globalization;
using System.IO;
using System.Text;
using WorshipPad.Core.Enums;
using WorshipPad.Core.Helpers;
using WorshipPad.Core.Models;

namespace WorshipPad.Core.Services;

public class PadLoaderService
{
    public IReadOnlyList<PadButton> LoadPads(
        string folderPath)
    {
        var pads = new List<PadButton>();

        if (!Directory.Exists(folderPath))
            return pads;


        // =====================================================
        // ARQUIVOS DE ÁUDIO
        // =====================================================

        var files =
            Directory
                .EnumerateFiles(
                    folderPath,
                    "*.*",
                    SearchOption.AllDirectories)
                .Where(IsAudioFile)
                .OrderBy(
                    file => Path.GetFileName(file),
                    StringComparer.OrdinalIgnoreCase);


        foreach (var file in files)
        {
            var fileName =
                Path.GetFileNameWithoutExtension(file);


            // =================================================
            // TENTA IDENTIFICAR COMO PAD
            // =================================================

            if (TryGetPadNote(
                fileName,
                out var note))
            {
                pads.Add(
                    new PadButton
                    {
                        Note = note,

                        DisplayName =
                            PadHelper.ToDisplayName(note),

                        AudioPath = file,

                        IsLooping = true
                    });

                continue;
            }


            // =================================================
            // ÁUDIO COMUM
            // =================================================

            pads.Add(
                new PadButton
                {
                    Note = null,

                    DisplayName =
                        FormatAudioName(fileName),

                    AudioPath = file,

                    IsLooping = true
                });
        }


        return pads;
    }


    // =========================================================
    // IDENTIFICAR NOTA
    // =========================================================

    private static bool TryGetPadNote(
        string fileName,
        out PadNote note)
    {
        note = default;


        if (string.IsNullOrWhiteSpace(fileName))
            return false;


        // -----------------------------------------------------
        // Normaliza apenas para identificar a nota.
        //
        // Exemplos:
        //
        // a_sharp
        // a_sharp_minor
        // a_sharp_major
        // a_minor
        // a_major
        //
        // viram:
        //
        // a_sharp
        // a_sharp
        // a_sharp
        // a
        // a
        // -----------------------------------------------------

        var normalized =
            fileName
                .Trim()
                .ToLowerInvariant();


        // -----------------------------------------------------
        // Remove informações de modo.
        // -----------------------------------------------------

        normalized =
            normalized
                .Replace("_minor", "")
                .Replace("_major", "")
                .Replace(" minor", "")
                .Replace(" major", "");


        normalized =
            normalized.Trim();


        // -----------------------------------------------------
        // Tenta o nome exato.
        //
        // a_sharp -> ASharp
        // a_flat  -> AFlat
        // etc.
        // -----------------------------------------------------

        var enumName =
            normalized
                .Replace("_", "");


        if (Enum.TryParse<PadNote>(
                enumName,
                true,
                out note))
        {
            return true;
        }


        // -----------------------------------------------------
        // Tenta também apenas a primeira parte da nota.
        //
        // Isso permite reconhecer nomes como:
        //
        // a_sharp_extra
        // a_minor
        // a_sharp_minor
        // -----------------------------------------------------

        var parts =
            normalized.Split(
                '_',
                StringSplitOptions.RemoveEmptyEntries);


        if (parts.Length > 0)
        {
            var noteName =
                parts[0];


            if (parts.Length >= 2 &&
                parts[1].Equals(
                    "sharp",
                    StringComparison.OrdinalIgnoreCase))
            {
                noteName += "Sharp";
            }


            if (Enum.TryParse<PadNote>(
                noteName,
                true,
                out note))
            {
                return true;
            }
        }


        // -----------------------------------------------------
        // Última tentativa:
        //
        // a-sharp
        // a sharp
        // a_sharp
        // -----------------------------------------------------

        var cleaned =
            normalized
                .Replace("-", "")
                .Replace(" ", "")
                .Replace("_", "");


        if (Enum.TryParse<PadNote>(
            cleaned,
            true,
            out note))
        {
            return true;
        }


        return false;
    }


    // =========================================================
    // VERIFICAR EXTENSÃO
    // =========================================================

    private static bool IsAudioFile(
        string file)
    {
        var extension =
            Path.GetExtension(file);


        return
            extension.Equals(
                ".mp3",
                StringComparison.OrdinalIgnoreCase)

            ||

            extension.Equals(
                ".m4a",
                StringComparison.OrdinalIgnoreCase)

            ||

            extension.Equals(
                ".wav",
                StringComparison.OrdinalIgnoreCase);
    }


    // =========================================================
    // NOME DO ÁUDIO
    // =========================================================

    private static string FormatAudioName(
        string fileName)
    {
        var name =
            fileName
                .Replace("_", " ")
                .Replace("-", " ")
                .Trim();


        if (string.IsNullOrWhiteSpace(name))
            return "Áudio";


        // -----------------------------------------------------
        // Primeira letra maiúscula
        // -----------------------------------------------------

        if (name.Length > 1)
        {
            name =
                char.ToUpper(name[0]) +
                name.Substring(1);
        }
        else
        {
            name =
                name.ToUpper();
        }


        // -----------------------------------------------------
        // Limite de 50 caracteres
        // -----------------------------------------------------

        const int maxLength = 50;


        if (name.Length > maxLength)
        {
            name =
                name.Substring(
                    0,
                    maxLength - 3)
                + "...";
        }


        return name;
    }
}
