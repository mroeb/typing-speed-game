using System.Windows;
using WpfApp1.ViewModels;

namespace WpfApp1;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var window = new MainWindow(CompositionRoot.CreateViewModel());

        MainWindow = window;
        window.Show();
    }
}
