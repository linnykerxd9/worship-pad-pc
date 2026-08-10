using CommunityToolkit.Mvvm.ComponentModel;
using WorshipPad.Core.Enums;

namespace WorshipPad.Core.Models;

public partial class PadButton : ObservableObject
{
    // =========================================================
    // NOTA
    // =========================================================

    // Será null quando o arquivo for um áudio comum.
    public PadNote? Note { get; set; }


    // =========================================================
    // NOME
    // =========================================================

    public string DisplayName { get; set; } = "";


    // =========================================================
    // ARQUIVO
    // =========================================================

    public string AudioPath { get; set; } = "";


    // =========================================================
    // ESTADO
    // =========================================================

    [ObservableProperty]
    private bool isPlaying;


    // =========================================================
    // LOOP
    // =========================================================

    // Por padrão, todo áudio começa com loop ativado.
    [ObservableProperty]
    private bool isLooping = true;


    // =========================================================
    // TIPO
    // =========================================================

    // True = pad de nota
    // False = áudio comum
    public bool IsNote =>
        Note.HasValue;
}
