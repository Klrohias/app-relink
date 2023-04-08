using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace AppRelink;

public class AppEntry : INotifyPropertyChanged
{
    private string _appName = "";

    public string AppName
    {
        get => _appName;
        set => SetValueAndNotify(ref _appName, value);
    }

    public bool LinkStatus => LinkEntries.All(x => x.LinkStatus);
    public ObservableCollection<LinkEntry> LinkEntries { get; set; } = new();

    public void Apply(Action<Action> startupAction)
    {
        if (LinkStatus) return;
        foreach (var linkEntry in LinkEntries)
        {
            if (linkEntry is {LinkStatus: false, TaskRunning: false})
            {
                startupAction.Invoke(linkEntry.Apply);
            }
        }
    }

    public void Apply() => Apply(doIt => doIt.Invoke());

    public void Recover(Action<Action> startupAction)
    {
        foreach (var linkEntry in LinkEntries)
        {
            if (linkEntry is {TaskRunning: false})
            {
                startupAction.Invoke(linkEntry.Recover);
            }
        }
    }

    public void Recover() => Recover(doIt => doIt.Invoke());

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
        foreach (var name in propertyNames)
        {
            OnPropertyChanged(name);
        }
    }

    #endregion
}