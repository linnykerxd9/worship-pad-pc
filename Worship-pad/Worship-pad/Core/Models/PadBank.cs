using CommunityToolkit.Mvvm.ComponentModel;

public partial class PadBank : ObservableObject
{
    public string Name { get; set; } = "";

    public string FolderPath { get; set; } = "";

    [ObservableProperty]
    private bool isSelected;
}