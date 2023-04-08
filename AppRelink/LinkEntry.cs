using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace AppRelink;

public class LinkEntry : INotifyPropertyChanged
{
    private string _sourceDirectory = "";
    private string _destinationDirectory = "";
    private string _lastError = "";
    private bool _taskRunning;

    public string SourceDirectory
    {
        get => _sourceDirectory;
        set => SetValueAndNotify(ref _sourceDirectory, value);
    }

    public string DestinationDirectory
    {
        get => _destinationDirectory;
        set => SetValueAndNotify(ref _destinationDirectory, value);
    }
    
    public string LastError
    {
        get => _lastError;
        set => SetValueAndNotify(ref _lastError, value, nameof(LastError), nameof(LinkStatus));
    }
    
    public bool LinkStatus
    {
        get
        {
            if (!Directory.Exists(_destinationDirectory)) return false;
            var fileInfo = new FileInfo(_sourceDirectory);
            return (long)fileInfo.Attributes != -1 && fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
        }
    }

    [JsonIgnore]
    public bool TaskRunning
    {
        get => _taskRunning;
        set => SetValueAndNotify(ref _taskRunning, value);
    }

    public void Apply()
    {
        if (LinkStatus || TaskRunning) return;
        TaskRunning = true;
        try
        {
            if (Directory.Exists(_sourceDirectory))
            {
                Utils.MoveTree(_sourceDirectory, _destinationDirectory);
            }

            Directory.CreateSymbolicLink(_sourceDirectory, _destinationDirectory);
            LastError = "Success";
        }
        catch (Exception e)
        {
            LastError = e.Message;
            throw;
        }
        finally
        {
            TaskRunning = false;
        }
    }

    public void Recover()
    {
        if (TaskRunning) return;
        TaskRunning = true;
        try
        {
            if (LinkStatus) Directory.Delete(_sourceDirectory);
            if (Directory.Exists(_destinationDirectory)) 
                Utils.MoveTree(_destinationDirectory, _sourceDirectory);
            LastError = "Success";
        }
        catch (Exception e)
        {
            LastError = e.Message;
            throw;
        }
        finally
        {
            TaskRunning = false;
        }
    }

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

    protected void SetValueAndNotify<T>(ref T field, T value, params string[] propertyNames)
    {
        field = value;
        foreach (var name in propertyNames)
        {
            OnPropertyChanged(name);
        }
    }

    #endregion
}