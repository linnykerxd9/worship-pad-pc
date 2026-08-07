using CommunityToolkit.Mvvm.ComponentModel;

public partial class PadBank : ObservableObject
{
    public string Name { get; set; } = "";

    public string FolderPath { get; set; } = "";
    public int PadCount { get; set; }
    [ObservableProperty]
    private bool isSelected;
    public string DisplayName => $"{Name} ({PadCount})";
}