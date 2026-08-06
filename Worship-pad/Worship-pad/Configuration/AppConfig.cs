namespace WorshipPad.Configuration;

public class AppConfig
{
    public string SelectedBank { get; set; } = "";

    public string SelectedDriver { get; set; } = "";

    public int UsbOutput { get; set; } = 15;

    public float Volume { get; set; } = 0.8f;

    public bool CrossFade { get; set; } = false;

    public float FadeTime { get; set; } = 2f;

    public bool RememberLastPad { get; set; } = true;

    public string LastPad { get; set; } = "C";
}