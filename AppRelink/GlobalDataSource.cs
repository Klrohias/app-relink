using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace AppRelink;

public class GlobalDataSource : INotifyPropertyChanged
{
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

    public static void Save() => File.WriteAllText(Configuration, JsonConvert.SerializeObject(_instance));

    #region Properties

    private ObservableCollection<AppEntry> _appEntries = new();

    public ObservableCollection<AppEntry> AppEntries
    {
        get => _appEntries;
        set => SetValueAndNotify(ref _appEntries, value);
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
        foreach (var name in propertyNames)
        {
            OnPropertyChanged(name);
        }
    }

    #endregion
}