using System.IO;
using System.Windows;
using System.Windows.Forms;
using AppRelink.Utils;
using EnumDialogResult = System.Windows.Forms.DialogResult;
using MessageBox = System.Windows.MessageBox;

namespace AppRelink.Windows;

public partial class AddLinkEntryWindow : Window
{
    private readonly AppEntry _appEntry;

    public AddLinkEntryWindow(AppEntry appEntry)
    {
        _appEntry = appEntry;
        InitializeComponent();
    }

    public LinkEntry Entry { get; } = new();


    private void OnCancelClicked(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OnOkClicked(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Entry.DestinationDirectory)
            || string.IsNullOrWhiteSpace(Entry.SourceDirectory)
            || !Directory.Exists(Entry.SourceDirectory))
        {
            MessageBox.Show("路径为空或目标路径不存在");
            return;
        }

        if (Directory.Exists(Entry.DestinationDirectory) &&
            Directory.GetFileSystemEntries(Entry.DestinationDirectory).Length != 0)
        {
            MessageBox.Show("目标路径必须为空");
            return;
        }

        _appEntry.LinkEntries.Add(Entry);
        GlobalDataSource.Save();
        Close();
    }

    private void OnBrowseClicked(object sender, RoutedEventArgs e)
    {
        var tag = Utilities.ResolveTag<string>(sender);

        var dialog = new FolderBrowserDialog();
        if (dialog.ShowDialog() == EnumDialogResult.Cancel) return;

        switch (tag)
        {
            case "SourceDirectory":
            {
                Entry.SourceDirectory = dialog.SelectedPath;
                break;
            }
            case "DestinationDirectory":
            {
                Entry.DestinationDirectory = dialog.SelectedPath;
                break;
            }
        }
    }
}