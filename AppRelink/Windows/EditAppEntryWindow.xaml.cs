using System;
using System.Windows;
using AppRelink.Utils;

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
        var entry = Utilities.ResolveDataContext<LinkEntry>(sender);

        if (MessageBox.Show("确定要移除这个重定向吗?", "Confirm", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.No)
            return;

        var task = new TaskModel
        {
            Type = TaskType.Recover,
            AffectedObject = entry
        };

        task.OnCompleted += () =>
        {
            _appEntry.LinkEntries.Remove(entry);
            GlobalDataSource.Save();
        };

        if (!GlobalDataSource.Instance.AddToTaskQueue(task)) Utilities.TaskConflictError();
    }

    private void OnSyncClicked(object sender, RoutedEventArgs e)
    {
        var entry = Utilities.ResolveDataContext<LinkEntry>(sender);
        if (!GlobalDataSource.Instance.AddToTaskQueue(new TaskModel
            {
                Type = TaskType.Apply,
                AffectedObject = entry
            }))
            Utilities.TaskConflictError();
    }

    private void OnAddClicked(object sender, RoutedEventArgs e)
    {
        new AddLinkEntryWindow(_appEntry).ShowDialog();
    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        GlobalDataSource.Save();
    }
}