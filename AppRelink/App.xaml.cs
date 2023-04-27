using System.Windows;

namespace AppRelink;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        GlobalDataSource.Instance.RunWorkerThread();
        base.OnStartup(e);
    }
}