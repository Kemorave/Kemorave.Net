using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfTestApp
{
    /// <summary>
    /// Interaction logic for WinApi.xaml
    /// </summary>
    public partial class WinApi : UserControl
    {
        public WinApi()
        {
            InitializeComponent();
        }

        private void LoadShell(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(BrowsePathTexBox.Text))
                {
                    throw new ArgumentNullException(nameof(BrowsePathTexBox.Text));
                }
                if (!System.IO.Directory.Exists(BrowsePathTexBox.Text))
                {
                    throw new InvalidOperationException();
                }
                Kemorave.Win.Shell.ShellItem shellitem = Kemorave.Win.Shell.ShellItem.FromParsingName(BrowsePathTexBox.Text);
                MainFrame.Navigate(new ShellDirectoryViewPage(shellitem));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

    }
}