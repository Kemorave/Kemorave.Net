using System;
using System.Windows;
using System.Windows.Controls;
namespace Kemorave.Wpf
{
    [TemplatePart(Name = "PART_AnimatedScrollViewer", Type = typeof(AnimatedScrollViewer))]
    public class AnimatedListBox : ListBox
    {
        public AnimatedScrollViewer ScrollViewer { get; private set; }
        public static readonly DependencyProperty ScrollToSelectedItemProperty = DependencyProperty.Register("ScrollToSelectedItem", typeof(bool), typeof(AnimatedListBox), new PropertyMetadata(false));
        public static readonly DependencyProperty SelectedIndexOffsetProperty = DependencyProperty.Register("SelectedIndexOffset", typeof(int), typeof(AnimatedListBox), new PropertyMetadata(0));

        static AnimatedListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedListBox), new FrameworkPropertyMetadata(typeof(AnimatedListBox)));
        }
        public void IncreamentHorizontalScrollOffset(double delta)
        {
            ScrollViewer.TargetHorizontalOffset = (ScrollViewer.NormalizeScrollPos(ScrollViewer.HorizontalOffset + delta, Orientation.Horizontal));

        }
        public void IncreamentVerticalScrollOffset(double delta)
        {
            ScrollViewer.TargetVerticalOffset = ScrollViewer.NormalizeScrollPos(ScrollViewer.VerticalScrollOffset + delta, Orientation.Vertical);

        }
        public void SetHorizontalScrollOffset(double HorizontalScrollOffset)
        {
            ScrollViewer.TargetHorizontalOffset = (ScrollViewer.NormalizeScrollPos(HorizontalScrollOffset, Orientation.Horizontal));
        }
        public void SetVerticalScrollOffset(double VerticalScrollOffset)
        {
            ScrollViewer.TargetVerticalOffset = ScrollViewer.NormalizeScrollPos(VerticalScrollOffset, Orientation.Vertical);
        }
        private void AnimatedListBox_LayoutUpdated(object sender, EventArgs e)
        {
            this.UpdateScrollPosition(sender);
        }

        private void AnimatedListBox_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateScrollPosition(sender);
        }

        private void AnimatedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateScrollPosition(sender);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (GetTemplateChild("PART_AnimatedScrollViewer") is AnimatedScrollViewer templateChild)
            {
                this.ScrollViewer = templateChild;
            }
            //this.PreviewKeyDown += ScrollViewer.OnPreviewKeyDownEvent;
            SelectionChanged += new SelectionChangedEventHandler(this.AnimatedListBox_SelectionChanged);
            Loaded += new RoutedEventHandler(this.AnimatedListBox_Loaded);
            LayoutUpdated += new EventHandler(this.AnimatedListBox_LayoutUpdated);
        }

        public void UpdateScrollPosition(object sender)
        {
            AnimatedListBox box = (AnimatedListBox)sender;
            if ((box != null) && box.ScrollToSelectedItem)
            {
                double num = 0.0;
                for (int i = 0; i < (box.SelectedIndex + box.SelectedIndexOffset); i++)
                {
                    if (box.ItemContainerGenerator.ContainerFromItem(box.Items[i]) is ListBoxItem item)
                    {
                        num += item.ActualHeight;
                    }
                }
                this.ScrollViewer.TargetVerticalOffset = num;
            }
        }

        public bool ScrollToSelectedItem
        {
            get =>
                ((bool)GetValue(ScrollToSelectedItemProperty));
            set =>
                SetValue(ScrollToSelectedItemProperty, value);
        }

        public int SelectedIndexOffset
        {
            get =>
                ((int)GetValue(SelectedIndexOffsetProperty));
            set =>
                SetValue(SelectedIndexOffsetProperty, value);
        }
    }


}