using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Data;
using AppRelink.Utils;
using Newtonsoft.Json;

namespace AppRelink;

public class GlobalDataSource : INotifyPropertyChanged
{
    #region Singlelon

    private const string Configuration = "AppConf.json";

    public static GlobalDataSource Instance
    {
        get
        {
            if (_instance != null) return _instance;

            return _instance = File.Exists(Configuration)
                ? JsonConvert.DeserializeObject<GlobalDataSource>(File.ReadAllText(Configuration))!
                : new GlobalDataSource();
        }
    }

    private static GlobalDataSource? _instance;

    public static void Save()
    {
        File.WriteAllText(Configuration, JsonConvert.SerializeObject(_instance));
    }


    public GlobalDataSource()
    {
        var taskQueueLock = new object();
        BindingOperations.EnableCollectionSynchronization(TaskQueue, taskQueueLock);
    }

    #endregion

    #region Properties

    private bool _running = true;
    private SyncObserverCollection<AppEntry> _appEntries = new();

    public SyncObserverCollection<AppEntry> AppEntries
    {
        get => _appEntries;
        set => SetValueAndNotify(ref _appEntries, value);
    }

    [JsonIgnore] public ObservableCollection<TaskModel> TaskQueue { get; set; } = new();

    #endregion

    #region Methods

    public bool AddToTaskQueue(TaskModel model)
    {
        lock (TaskQueue)
        {
            if (TaskQueue.Any(x => x.AffectedObject == model.AffectedObject)) return false;
            TaskQueue.Add(model);
        }

        return true;
    }

    public bool CanAddToTaskQueue(AppEntry appEntry)
    {
        lock (TaskQueue)
        {
            if (TaskQueue.Any(x => appEntry.LinkEntries.Contains(x.AffectedObject))) return false;
        }

        return true;
    }

    public void RunWorkerThread()
    {
        new Thread(WorkerThread).Start();
    }

    public void StopWorkerThread()
    {
        _running = false;
    }

    private void WorkerThread()
    {
        while (_running)
        {
            lock (TaskQueue)
            {
                if (TaskQueue.Count > 0)
                {
                    var task = TaskQueue[0];
                    TaskQueue.RemoveAt(0);
                    task.Run();
                }
            }

            Thread.Sleep(1000);
        }
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