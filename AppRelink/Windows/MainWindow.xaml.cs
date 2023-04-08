using System.Threading.Tasks;
using System.Windows;

namespace AppRelink.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        DataContext = GlobalDataSource.Instance;
        InitializeComponent();
    }

    private void OnAddClicked(object sender, RoutedEventArgs e) => new AddAppEntryWindow().ShowDialog();

    private void OnEditClicked(object sender, RoutedEventArgs e)
    {
        var entry = Utils.ResolveDataContext<AppEntry>(sender);
        new EditAppEntryWindow(entry).ShowDialog();
    }

    private async void OnDeleteClicked(object sender, RoutedEventArgs e)
    {
        var entry = Utils.ResolveDataContext<AppEntry>(sender);
        if (MessageBox.Show("Do you want to delete this entry?", "Confirm", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.No)
        {
            return;
        }

        try
        {
            await Task.Run(() => entry.Recover());

            GlobalDataSource.Instance.AppEntries.Remove(entry);
            GlobalDataSource.Save();
        }
        catch
        {
            // ignored exception
        }
    }
}
