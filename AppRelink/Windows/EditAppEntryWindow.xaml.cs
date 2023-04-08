using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AppRelink.Windows;

public partial class EditAppEntryWindow : Window
{
    private readonly AppEntry _appEntry;

    public EditAppEntryWindow(AppEntry appEntry)
    {
        DataContext = _appEntry = appEntry;
        InitializeComponent();
    }

    private async void OnDeleteClicked(object sender, RoutedEventArgs e)
    {
        var entry = Utils.ResolveDataContext<LinkEntry>(sender);

        if (MessageBox.Show("Do you want to delete this entry?", "Confirm", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.No)
        {
            return;
        }

        try
        {
            await Task.Run(() => entry.Recover());

            _appEntry.LinkEntries.Remove(entry);
            GlobalDataSource.Save();
        }
        catch
        {
            // ignored exception
        }
    }

    private void OnSyncClicked(object sender, RoutedEventArgs e)
    {
        var entry = Utils.ResolveDataContext<LinkEntry>(sender);
        new Thread(() =>
        {
            try
            {
                entry.Apply();
            }
            catch
            {
                // ignored exception
            }
        }).Start();
    }

    private void OnAddClicked(object sender, RoutedEventArgs e)
    {
        new AddLinkEntryWindow(_appEntry).ShowDialog();
    }
}