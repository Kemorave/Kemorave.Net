
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Kemorave.Wpf
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Kemorave.Wpf"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Kemorave.Wpf;assembly=Kemorave.Wpf"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:AnimatedScrollViewer/>
    ///
    /// </summary> 
    [TemplatePart(Name = "PART_HorizontalScrollBar", Type = typeof(ScrollBar)), TemplatePart(Name = "PART_VerticalScrollBar", Type = typeof(ScrollBar))]
    public class AnimatedScrollViewer : ScrollViewer
    {
        static AnimatedScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedScrollViewer), new FrameworkPropertyMetadata(typeof(AnimatedScrollViewer)));
        }

        private ScrollBar _VerticalScrollBar;
        private ScrollBar _HorizontalScrollBar;



        public bool KeyScrollable
        {
            get { return (bool)GetValue(KeyScrollableProperty); }
            set { SetValue(KeyScrollableProperty, value); }
        }



        public static bool GetKeyScrollable(DependencyObject obj)
        {
            return (bool)obj.GetValue(KeyScrollableProperty);
        }

        public static void SetKeyScrollable(DependencyObject obj, bool value)
        {
            obj.SetValue(KeyScrollableProperty, value);
        }

        // Using a DependencyProperty as the backing store for KeyScrollable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyScrollableProperty =
                        DependencyProperty.RegisterAttached("KeyScrollable", typeof(bool), typeof(AnimatedScrollViewer), new PropertyMetadata(true));




        public static readonly DependencyProperty TargetVerticalOffsetProperty
        = DependencyProperty.Register("TargetVerticalOffset", typeof(double), typeof(AnimatedScrollViewer),
                        new PropertyMetadata(0.0, new PropertyChangedCallback(OnTargetVerticalOffsetChanged)));

        private static void OnTargetVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimatedScrollViewer objectToScroll = (AnimatedScrollViewer)d;
            objectToScroll._VerticalScrollBar.Value = (double)e.NewValue;
            objectToScroll.AnimateV(objectToScroll._VerticalScrollBar.Value);
        }


        public static readonly DependencyProperty TargetHorizontalOffsetProperty
                        = DependencyProperty.Register("TargetHorizontalOffset", typeof(double), typeof(AnimatedScrollViewer),
                                        new PropertyMetadata(0.0, new PropertyChangedCallback(OnTargetHorizontalOffsetChanged)));
        private static void OnTargetHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AnimatedScrollViewer objectToScroll = (AnimatedScrollViewer)d;

            objectToScroll._HorizontalScrollBar.Value = (double)e.NewValue;

            objectToScroll.AnimateH(objectToScroll._HorizontalScrollBar.Value);
        }


        private static readonly DependencyProperty HorizontalScrollOffsetProperty
        = DependencyProperty.Register("HorizontalScrollOffset", typeof(double), typeof(AnimatedScrollViewer),
                        new PropertyMetadata(0.0, new PropertyChangedCallback(OnHorizontalScrollOffsetChanged)));

        private static void OnHorizontalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AnimatedScrollViewer)d).ScrollToHorizontalOffset((double)e.NewValue);
        }


        private static readonly DependencyProperty VerticalScrollOffsetProperty
        = DependencyProperty.Register("VerticalScrollOffset", typeof(double), typeof(AnimatedScrollViewer),
                        new PropertyMetadata(0.0, new PropertyChangedCallback(OnVerticalScrollOffsetChanged)));
        private static void OnVerticalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AnimatedScrollViewer)d).ScrollToVerticalOffset((double)e.NewValue);
        }



        public static readonly DependencyProperty ScrollingDurationProperty
        = DependencyProperty.Register("ScrollingDuration", typeof(Duration), typeof(AnimatedScrollViewer),
                        new PropertyMetadata(new Duration(new TimeSpan(0, 0, 0, 0, 200))));




     

        private void AnimateH(double val)
        {
           Helper.ControlHelper.StartDoubleAnimation(this, HorizontalScrollOffsetProperty, val, ScrollingDuration, HandoffBehavior.SnapshotAndReplace);
        }
        private void AnimateV(double val)
        {
            Helper.ControlHelper.StartDoubleAnimation(this, VerticalScrollOffsetProperty, val, ScrollingDuration, HandoffBehavior.SnapshotAndReplace);
        }

        private void CustomPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double num2 = VerticalOffset - (e.Delta + ((e.Delta) > 0 ? 200 : -200));
            if (num2 < 0.0)
            {
                TargetVerticalOffset = 0.0;
            }
            else if (num2 > ScrollableHeight)
            {
                TargetVerticalOffset = ScrollableHeight;
            }
            else
            {
                TargetVerticalOffset = num2;
            }
            e.Handled = true;
        }


        public double NormalizeScrollPos(double scrollChange, Orientation o)
        {
            double scrollableWidth = scrollChange;
            if (scrollChange < 0.0)
            {
                scrollableWidth = 0.0;
            }
            if ((o == Orientation.Vertical) && (scrollChange > ScrollableHeight))
            {
                return ScrollableHeight;
            }
            if ((o == Orientation.Horizontal) && (scrollChange > ScrollableWidth))
            {
                scrollableWidth = ScrollableWidth;
            }
            return scrollableWidth;
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _VerticalScrollBar = GetTemplateChild("PART_VerticalScrollBar") as ScrollBar;


            _HorizontalScrollBar = GetTemplateChild("PART_HorizontalScrollBar") as ScrollBar;

            PreviewKeyDown += OnPreviewKeyDownEvent;
            PreviewMouseWheel += new MouseWheelEventHandler(this.CustomPreviewMouseWheel);
        }
        public new void ScrollToEnd()
        {
            TargetVerticalOffset = ScrollableHeight;
        }
        public new void ScrollToTop()
        {
            TargetVerticalOffset = 0;
        }
        public void OnPreviewKeyDownEvent(object sender, KeyEventArgs e)
        {
            //if (KeyScrollable)
            //{
            //	switch (e.Key)
            //	{
            //		case Key.Down:
            //			{
            //				ScrollDown();
            //				e.Handled = true;
            //				break;
            //			}
            //		case Key.Up:
            //			{
            //				ScrollUp();
            //				e.Handled = true;
            //				break;
            //			}
            //		case Key.Left:
            //			{
            //				ScrollLeft();
            //				e.Handled = true;
            //				break;
            //			}
            //		case Key.Right:
            //			{
            //				ScrollRight();
            //				e.Handled = true;
            //				break;
            //			}
            //		default: break;
            //	}
            //	NormalizeScrollPos();
            //}
        }
        public void ScrollUp()
        {
            TargetVerticalOffset -= ScrollDelta;
        }
        public void ScrollDown()
        {
            TargetVerticalOffset += ScrollDelta;
        }
        public void ScrollLeft()
        {
            TargetHorizontalOffset -= ScrollDelta;
        }
        public void ScrollRight()
        {
            TargetHorizontalOffset += ScrollDelta;
        }
#pragma warning disable IDE0051 // Remove unused private members
        private void NormalizeScrollPos()
#pragma warning restore IDE0051 // Remove unused private members
        {
            if (TargetHorizontalOffset > ScrollableWidth)
            {
                TargetHorizontalOffset = ScrollableWidth;
            }
            if (TargetHorizontalOffset < 0)
            {
                TargetHorizontalOffset = 0;
            }
            if (TargetVerticalOffset > ScrollableHeight)
            {
                TargetVerticalOffset = ScrollableHeight;
            }
            if (TargetVerticalOffset < 0)
            {
                TargetVerticalOffset = 0;
            }
        }

        public double ScrollDelta { get; set; } = 50;
        public double TargetVerticalOffset
        {
            get =>
                            ((double)GetValue(TargetVerticalOffsetProperty));
            set =>
                            SetValue(TargetVerticalOffsetProperty, value);
        }

        public double TargetHorizontalOffset
        {
            get =>
                            ((double)GetValue(TargetHorizontalOffsetProperty));
            set =>
                            SetValue(TargetHorizontalOffsetProperty, value);
        }

        public double HorizontalScrollOffset
        {
            get =>
                            ((double)GetValue(HorizontalScrollOffsetProperty));
            set =>
                            SetValue(HorizontalScrollOffsetProperty, value);
        }

        public double VerticalScrollOffset
        {
            get =>
                            ((double)GetValue(VerticalScrollOffsetProperty));
            set =>
                            SetValue(VerticalScrollOffsetProperty, value);
        }

        public Duration ScrollingDuration
        {
            get =>
                            (Duration)GetValue(ScrollingDurationProperty);
            set =>
                            SetValue(ScrollingDurationProperty, value);
        }
    }
}