using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Kemorave.Win.Shell;

namespace WpfTestApp
{
    /// <summary>
    /// Interaction logic for ShellDirectoryViewPage.xaml
    /// </summary>
    public partial class ShellDirectoryViewPage : Page
    {
        public ShellDirectoryViewPage(Kemorave.Win.Shell.ShellItem shellItem)
        {
            Title = shellItem.Name;
            //DataContext = shellItem;
            InitializeComponent();
            LoadShellObjects(shellItem);
        }

        private void LoadShellObjects(ShellItem shellitem)
        {
            this.Cursor = Cursors.Wait;
            Task.Run(() =>
            {
                try
                {
                    if (shellitem.ShellInfo is Microsoft.WindowsAPICodePack.Shell.ShellContainer container)
                    {
                        List<ShellItem> items =new List<ShellItem>( Kemorave.Win.Shell.ShellItem.FromContainer(container) );
                        Dispatcher.Invoke(() => { ShellItemsListBox.ItemsSource = items; });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
                finally
                {
                    Dispatcher.Invoke(() => { this.Cursor = Cursors.Arrow; }); 
                }
            });

        }

        private void OnShellObjectSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnObjectClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem lb && lb.DataContext is Kemorave.Win.Shell.ShellItem si && si.ShellInfo is Microsoft.WindowsAPICodePack.Shell.ShellContainer)
            {
                try
                {
                    NavigationService.Navigate(new ShellDirectoryViewPage(si));
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
