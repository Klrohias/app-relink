using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Win32;
using MessageBox = System.Windows.MessageBox;

namespace AppRelink;

public partial class AddAppEntryWindow : Window
{
    public AddAppEntryWindow()
    {
        InitializeComponent();
    }

    public AppEntry Entry { get; } = new();

    private void OnCancelClicked(object sender, RoutedEventArgs e) => Close();

    private void OnOkClicked(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Entry.AppName))
        {
            MessageBox.Show("Application Name cannot be empty.");
            return;
        }

        GlobalDataSource.Instance.AppEntries.Add(Entry);
        GlobalDataSource.Save();
        Close();
    }
}