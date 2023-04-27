using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;

namespace AppRelink.Utils;

public sealed class SyncObserverCollection<T> : ObservableCollection<T>
{
    public SyncObserverCollection(bool sync = true)
    {
        CollectionChanged += CollectionChangedHandler;
        var collectionLock = new object();
        if (sync) BindingOperations.EnableCollectionSynchronization(this, collectionLock);
    }

    private void AddObserver(object? o)
    {
        if (o is INotifyPropertyChanged typedObject) typedObject.PropertyChanged += ObserverMethod;
    }

    private void ObserverMethod(object? sender, PropertyChangedEventArgs e)
    {
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender,
            sender,
            IndexOf((T) sender!)));
    }

    private void RemoveObserver(object? o)
    {
        if (o is INotifyPropertyChanged typedObject) typedObject.PropertyChanged -= ObserverMethod;
    }

    private void CollectionChangedHandler(object? sender
        , NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
            foreach (var item in e.NewItems)
                AddObserver(item);

        if (e.OldItems != null)
            foreach (var item in e.OldItems)
                RemoveObserver(item);
    }
}