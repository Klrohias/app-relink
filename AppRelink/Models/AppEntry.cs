using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using AppRelink.Utils;

namespace AppRelink;

public class AppEntry : INotifyPropertyChanged
{
    public AppEntry()
    {
        LinkEntries.CollectionChanged += (sender, args) => { OnPropertyChanged(nameof(LinkStatus)); };
    }

    #region Properties

    private string _appName = "";

    public string AppName
    {
        get => _appName;
        set => SetValueAndNotify(ref _appName, value);
    }

    public bool LinkStatus => LinkEntries.All(x => x.LinkStatus);
    public SyncObserverCollection<LinkEntry> LinkEntries { get; set; } = new();

    #endregion

    #region Methods

    public override string ToString()
    {
        return _appName;
    }

    public bool Apply()
    {
        if (!GlobalDataSource.Instance.CanAddToTaskQueue(this)) return false;

        foreach (var linkEntry in LinkEntries)
            GlobalDataSource.Instance.AddToTaskQueue(new TaskModel
            {
                AffectedObject = linkEntry,
                Type = TaskType.Apply
            });

        return true;
    }

    public bool Recover()
    {
        if (!GlobalDataSource.Instance.CanAddToTaskQueue(this)) return false;

        foreach (var linkEntry in LinkEntries)
            GlobalDataSource.Instance.AddToTaskQueue(new TaskModel
            {
                AffectedObject = linkEntry,
                Type = TaskType.Recover
            });

        return true;
    }

    #endregion

    #region INotifyPropertyChanged Impls

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected void SetValueAndNotify<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        field = value;
        OnPropertyChanged(propertyName);
    }

    protected void SetValueAndNotify<T>(ref T field, T value, string[] propertyNames)
    {
        field = value;
        foreach (var name in propertyNames) OnPropertyChanged(name);
    }

    #endregion
}