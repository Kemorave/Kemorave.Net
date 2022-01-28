using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Kemorave.Wpf.Helper
{
    public class ToolTipHelper
    {
        public static readonly DependencyProperty MousePointToolTipProperty =
        DependencyProperty.RegisterAttached("MousePointToolTip", typeof(object), typeof(ToolTipHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, MousePointToolTipChanged));
        public static void SetMousePointToolTip(DependencyObject control, object st)
        {
            control.SetValue(MousePointToolTipProperty, st);
        }
        public static object GetMousePointToolTip(DependencyObject control)
        {
            var val = control.GetValue(MousePointToolTipProperty);

            return val;
            // return false;
        }
        private static void MousePointToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs x)
        {
            Debug.WriteLine(d + " => MousePointToolTip");
            if (d is FrameworkElement Element)
            {
                if (x.NewValue is object content)
                {
                    var tt = new ToolTip();
                    Element.ToolTip = tt;
                    tt.Content = content;
                    tt.Placement = PlacementMode.Relative;
                    Element.MouseMove += TrackOnMouseMove;
                }
                else
                {
                    Element.MouseMove -= TrackOnMouseMove;
                }
            }
        }

        private static void TrackOnMouseMove(object sender, MouseEventArgs e)
        {
            if (sender == null)
                return;
            if ((sender as FrameworkElement)?.ToolTip is ToolTip toolTip)
            {
                Debug.WriteLine(toolTip.Content);
                toolTip.Placement = PlacementMode.Relative;
                toolTip.HorizontalOffset = e.GetPosition(sender as FrameworkElement).X;

                toolTip.VerticalOffset = e.GetPosition(sender as FrameworkElement).Y;
            }
        }
    }
}

