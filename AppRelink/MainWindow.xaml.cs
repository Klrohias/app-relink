using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AppRelink
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = GlobalDataSource.Instance;
            InitializeComponent();
        }

        private void OnAddClicked(object sender, RoutedEventArgs e) => new AddAppEntryWindow().ShowDialog();

        private void OnEditClicked(object sender, RoutedEventArgs e)
        {
            var entry = Utils.ResolveDataContext<AppEntry>(sender);
            new EditAppEntryWindow(entry).ShowDialog();
        }

        private async void OnDeleteClicked(object sender, RoutedEventArgs e)
        {
            var entry = Utils.ResolveDataContext<AppEntry>(sender);
            if (MessageBox.Show("Do you want to delete this entry?", "Confirm", MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.No)
            {
                return;
            }

            try
            {
                await Task.Run(() => entry.Recover());

                GlobalDataSource.Instance.AppEntries.Remove(entry);
                GlobalDataSource.Save();
            }
            catch
            {
                // ignored exception
            }
        }
    }
}