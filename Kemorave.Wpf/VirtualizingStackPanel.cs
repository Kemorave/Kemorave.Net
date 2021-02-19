using System.Windows.Controls;

namespace Kemorave.Wpf
{
 public class VirtualizingStackPanel : System.Windows.Controls.VirtualizingStackPanel
 {
  protected override void OnCleanUpVirtualizedItem(CleanUpVirtualizedItemEventArgs e)
  {
   if (e.UIElement is ListBoxItem ListBoxItem)
   {
    if (ListBoxItem.IsSelected)
    {
     e.Cancel = true;
     e.Handled = true;
    }
   }
   else if (e.UIElement is TreeViewItem TreeViewItem)
   {
    if (TreeViewItem.IsSelected)
    {
     e.Cancel = true;
     e.Handled = true;
    }
   }
   base.OnCleanUpVirtualizedItem(e);
  }
 }
}