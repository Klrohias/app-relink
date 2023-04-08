using System.IO;
using System.Windows;
using FolderBrowserDialog = System.Windows.Forms.FolderBrowserDialog;
using EnumDialogResult = System.Windows.Forms.DialogResult;
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


    private void OnCancelClicked(object sender, RoutedEventArgs e) => Close();

    private void OnOkClicked(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Entry.DestinationDirectory)
            || string.IsNullOrWhiteSpace(Entry.SourceDirectory)
            || !Directory.Exists(Entry.SourceDirectory))
        {
            MessageBox.Show("The path of directory is empty or directory does not exists.");
            return;
        }

        _appEntry.LinkEntries.Add(Entry);
        GlobalDataSource.Save();
        Close();
    }

    private void OnBrowseClicked(object sender, RoutedEventArgs e)
    {
        var tag = Utils.ResolveTag<string>(sender);
        var dialog = new FolderBrowserDialog();
        if (dialog.ShowDialog() == EnumDialogResult.Cancel)
        {
            return;
        }
        
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