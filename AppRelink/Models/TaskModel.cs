using System;

namespace AppRelink;

public class TaskModel
{
    public TaskType Type { get; set; }
    public object? AffectedObject { get; set; }

    public string Summary => Type switch
    {
        TaskType.Apply => "Sync",
        TaskType.Recover => "Recover",
        TaskType.RemoveApplication => "Remove",
        _ => throw new ArgumentOutOfRangeException()
    } + " " + (AffectedObject ?? "");

    public event Action OnCompleted;

    public void Run()
    {
        switch (Type)
        {
            case TaskType.Recover:
                RunRecover();
                break;

            case TaskType.Apply:
                RunApply();
                break;

            case TaskType.RemoveApplication:
                RunRemoveApplication();
                break;
        }

        OnCompleted?.Invoke();
    }

    private void RunApply()
    {
        if (AffectedObject == null) return;

        var typedObject = (LinkEntry) AffectedObject;
        typedObject.Apply();
    }

    private void RunRecover()
    {
        if (AffectedObject == null) return;

        var typedObject = (LinkEntry) AffectedObject;
        typedObject.Recover();
    }

    private void RunRemoveApplication()
    {
        if (AffectedObject == null) return;

        var typedObject = (AppEntry) AffectedObject;
        GlobalDataSource.Instance.AppEntries.Remove(typedObject);
        GlobalDataSource.Save();
    }
}

public enum TaskType
{
    Recover,
    Apply,
    RemoveApplication
}