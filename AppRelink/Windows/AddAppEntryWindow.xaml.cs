using System.Windows;

namespace AppRelink.Windows;

public partial class AddAppEntryWindow : Window
{
    public AddAppEntryWindow()
    {
        InitializeComponent();
    }

    public AppEntry Entry { get; } = new();

    private void OnCancelClicked(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OnOkClicked(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Entry.AppName))
        {
            MessageBox.Show("应用程序名称不能为空");
            return;
        }

        GlobalDataSource.Instance.AppEntries.Add(Entry);
        GlobalDataSource.Save();
        Close();
    }
}