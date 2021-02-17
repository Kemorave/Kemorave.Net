using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Kemorave.Wpf.Helper
{
    public class SelectorHelper
    {

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached("SelectedItems", typeof(IList), typeof(SelectorHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, SelectedItemsPropertyChanged));
        private static void SelectedItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is System.Windows.Controls.Primitives.Selector Selector)
            {
                if (e.NewValue is IList)
                {
                    Selector.SelectionChanged -= Element_SelectionChanged;
                    Selector.SelectionChanged += Element_SelectionChanged;
                }
                else
                {
                    Selector.SelectionChanged -= Element_SelectionChanged;
                    if (d is System.Windows.Controls.Primitives.MultiSelector MultiSelector)
                    {
                        MultiSelector.UnselectAll();
                    }
                    if (d is ListBox ListBox)
                    {
                        ListBox.UnselectAll();
                    }
                }
            }
        }

        private static void Element_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GetSelectedItems(sender as DependencyObject) is IList list)
            {
                foreach (object item in e.AddedItems)
                {
                    list.Add(item);
                }
                foreach (object item in e.RemovedItems)
                {
                    list.Remove(item);
                }
            }
        }

        public static void SetSelectedItems(DependencyObject control, IList st)
        {
            control.SetValue(SelectedItemsProperty, st);
        }
        public static IList GetSelectedItems(DependencyObject control)
        {
            return control?.GetValue(SelectedItemsProperty) as IList;
        }


    }
}