using System;
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

    private void OnDeleteClicked(object sender, RoutedEventArgs e)
    {
        var entry = Utils.Utils.ResolveDataContext<LinkEntry>(sender);

        if (MessageBox.Show("Do you want to delete this entry?", "Confirm", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.No)
        {
            return;
        }

        var task = new TaskModel
        {
            Type = TaskType.Recover,
            AffectedObject = entry,
        };

        task.OnCompleted += () =>
        {
            _appEntry.LinkEntries.Remove(entry);
            GlobalDataSource.Save();
        };

        if (!GlobalDataSource.Instance.AddToTaskQueue(task))
        {
            Utils.Utils.TaskConflictError();
        }
    }

    private void OnSyncClicked(object sender, RoutedEventArgs e)
    {
        var entry = Utils.Utils.ResolveDataContext<LinkEntry>(sender);
        if (!GlobalDataSource.Instance.AddToTaskQueue(new TaskModel
            {
                Type = TaskType.Apply,
                AffectedObject = entry,
            }))
        {
            Utils.Utils.TaskConflictError();
        }
    }

    private void OnAddClicked(object sender, RoutedEventArgs e) => new AddLinkEntryWindow(_appEntry).ShowDialog();
    private void OnWindowClosed(object? sender, EventArgs e) => GlobalDataSource.Save();
}