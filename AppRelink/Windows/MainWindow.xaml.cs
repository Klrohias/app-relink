using System;
using System.Windows;
using AppRelink.Utils;

namespace AppRelink.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        DataContext = GlobalDataSource.Instance;
        InitializeComponent();
    }

    private void OnAddClicked(object sender, RoutedEventArgs e)
    {
        new AddAppEntryWindow().ShowDialog();
    }

    private void OnEditClicked(object sender, RoutedEventArgs e)
    {
        var entry = Utilities.ResolveDataContext<AppEntry>(sender);
        new EditAppEntryWindow(entry).ShowDialog();
    }

    private void OnDeleteClicked(object sender, RoutedEventArgs e)
    {
        var entry = Utilities.ResolveDataContext<AppEntry>(sender);
        if (MessageBox.Show("Do you want to delete this entry?", "Confirm", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.No)
            return;

        if (!entry.Recover())
        {
            Utilities.TaskConflictError();
            return;
        }

        if (!GlobalDataSource.Instance.AddToTaskQueue(new TaskModel
            {
                Type = TaskType.RemoveApplication,
                AffectedObject = entry
            }))
            Utilities.TaskConflictError();
    }

    private void OnSyncAllClicked(object sender, RoutedEventArgs e)
    {
        var entry = Utilities.ResolveDataContext<AppEntry>(sender);
        if (!entry.Apply()) Utilities.TaskConflictError();
    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        GlobalDataSource.Instance.StopWorkerThread();
    }
}