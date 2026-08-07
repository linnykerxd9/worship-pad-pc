using System.Windows;
using WorshipPad.Views;

namespace WorshipPad;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var window = new WorshipPad.Views.MainWindow();

        window.Show();
    }
}