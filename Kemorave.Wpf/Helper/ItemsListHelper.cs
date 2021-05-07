using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Kemorave.Wpf.Helper
{
    public class ItemsListHelper
    {
      

        public static readonly DependencyProperty IsExtendedSelectionProperty = DependencyProperty.RegisterAttached("IsExtendedSelection", typeof(bool), typeof(ItemsListHelper), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, IsExtendedSelectionProprtyChanged));
        private static void IsExtendedSelectionProprtyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListBox ListBox)
            {
                ListBox.UnselectAll();
                ListBox.Unloaded += (j, kj) => { ListBox.UnselectAll(); };
                if ((bool)e.NewValue == true)
                {
                    ListBox.SelectionMode = SelectionMode.Extended;
                }
                else
                {
                    ListBox.SelectionMode = SelectionMode.Single;
                }
            }
            if (d is DataGrid DataGrid)
            {
                DataGrid.UnselectAll();
                DataGrid.Unloaded += (j, kj) => { DataGrid.UnselectAll(); };
                if ((bool)e.NewValue == true)
                {
                    DataGrid.SelectionMode = DataGridSelectionMode.Extended;
                }
                else
                {
                    DataGrid.SelectionMode = DataGridSelectionMode.Single;
                }
            }
        }
        public static void SetIsExtendedSelection(DependencyObject control, bool st)
        {
            control.SetValue(IsExtendedSelectionProperty, st);
        }
        public static bool GetIsExtendedSelection(DependencyObject control)
        {
            var val = control.GetValue(IsExtendedSelectionProperty);
            if (val is bool)
                return (bool)val;
            return false;
        }




        public static readonly DependencyProperty IsMultipleSelectionProperty = DependencyProperty.RegisterAttached("IsMultipleSelection", typeof(bool), typeof(ItemsListHelper), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, IsMultipleSelectionProprtyChanged));
        private static void IsMultipleSelectionProprtyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListBox ListBox)
            {
                ListBox.UnselectAll();
                ListBox.Unloaded += (j, kj) => { ListBox.UnselectAll(); };
                if ((bool)e.NewValue == true)
                {
                    ListBox.SelectionMode = SelectionMode.Multiple;
                }
                else
                {
                    ListBox.SelectionMode = SelectionMode.Single;
                }
            }
            if (d is DataGrid DataGrid)
            {
                DataGrid.UnselectAll();
                DataGrid.Unloaded += (j, kj) => { DataGrid.UnselectAll(); };
                if ((bool)e.NewValue == true)
                {
                    DataGrid.SelectionMode = DataGridSelectionMode.Extended;
                }
                else
                {
                    DataGrid.SelectionMode = DataGridSelectionMode.Single;
                }
            }
        }
        public static void SetIsMultipleSelection(DependencyObject control, bool st)
        {
            control.SetValue(IsMultipleSelectionProperty, st);
        }
        public static bool GetIsMultipleSelection(DependencyObject control)
        {
            var val = control.GetValue(IsMultipleSelectionProperty);
            if (val is bool)
                return (bool)val;
            return false;
        }



        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached("SelectedItems", typeof(IList), typeof(ItemsListHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, SelectedItemsPropertyChanged));
        private static void SelectedItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid DataGrid)
            {
                if (e.NewValue is IList)
                {
                    DataGrid.SelectionChanged += Element_SelectionChanged;
                }
                else
                {
                    DataGrid.UnselectAll();
                    DataGrid.SelectionChanged -= Element_SelectionChanged;
                }
            }
            if (d is ListBox ListBox)
            {
                if (e.NewValue is IList)
                {
                    ListBox.SelectionChanged += Element_SelectionChanged;
                }
                else
                {
                    ListBox.UnselectAll();
                    ListBox.SelectionChanged -= Element_SelectionChanged;
                }
            }
        }

        private static void Element_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox ListBox)
            {
                if (GetSelectedItems(ListBox) is IList list)
                {
                    foreach (var item in e.AddedItems)
                    {
                        list.Add(item);
                    }
                    foreach (var item in e.RemovedItems)
                    {
                        list.Remove(item);
                    }
                }
            }
            if (sender is DataGrid DataGrid)
            {
                if (GetSelectedItems(DataGrid) is IList list)
                {
                    foreach (var item in e.AddedItems)
                    {
                        list.Add(item);
                    }
                    foreach (var item in e.RemovedItems)
                    {
                        list.Remove(item);
                    }
                }
            }
        }

        public static void SetSelectedItems(DependencyObject control, IList st)
        {
            control.SetValue(SelectedItemsProperty, st);
        }
        public static IList GetSelectedItems(DependencyObject control)
        {
            var val = control.GetValue(SelectedItemsProperty);
            if (val is IList)
                return (IList)val;
            return null;
        }

    }
}