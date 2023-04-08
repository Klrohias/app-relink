using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace AppRelink.Utils;

public static class Utils
{
    public static void MoveTree(string srcDir, string dstDir)
    {
        if (!Directory.Exists(dstDir)) Directory.CreateDirectory(dstDir);

        foreach (var filePath in Directory.EnumerateFiles(srcDir))
        {
            var fileName = Path.GetFileName(filePath);
            File.Move(filePath, Path.Combine(dstDir, fileName));
        }

        foreach (var dirPath in Directory.EnumerateDirectories(srcDir))
        {
            var dirName = Path.GetFileName(dirPath);
            var targetDirPath = Path.Combine(dstDir, dirName);
            if (!Directory.Exists(targetDirPath)) Directory.CreateDirectory(targetDirPath);

            MoveTree(dirPath, targetDirPath);
        }

        Directory.Delete(srcDir);
    }

    public static T ResolveDataContext<T>(object sender) => (T) ((FrameworkElement) sender).DataContext;

    public static T ResolveTag<T>(object sender) => (T) ((FrameworkElement) sender).Tag;

    public static async void SaveOnFinish(Task task)
    {
        try
        {
            await task;
        }
        catch
        {
            // ignored exception
        }
        finally
        {
            GlobalDataSource.Save();
        }
    }

    public static void TaskConflictError() => MessageBox.Show(
        "There is a task related to this entry, please wait for the task to complete first.",
        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
}