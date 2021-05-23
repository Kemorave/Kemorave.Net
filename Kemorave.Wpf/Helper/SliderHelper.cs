using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Kemorave.Wpf.Helper
{
    public class SliderHelper
    {
        public static readonly DependencyProperty MousePointValueProperty = DependencyProperty.RegisterAttached("MousePointValue", typeof(double), typeof(SliderHelper), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static void SetMousePointValue(DependencyObject control, double st)
        {
            control.SetValue(MousePointValueProperty, st);
        }
        public static double GetMousePointValue(DependencyObject control)
        {
            var val = control.GetValue(MousePointValueProperty);
            if (val is double)
                return (double)val;
            return 0.0;
        }

        public static readonly DependencyProperty EnableMousePointValueProperty =
        DependencyProperty.RegisterAttached("EnableMousePointValue", typeof(bool), typeof(SliderHelper), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, EnableMousePointValueChanged));
        public static void SetEnableMousePointValue(DependencyObject control, bool st)
        {
            control.SetValue(EnableMousePointValueProperty, st);
        }
        public static bool GetEnableMousePointValue(DependencyObject control)
        {
            var val = control.GetValue(EnableMousePointValueProperty);
            if (val is bool)
                return (bool)val;
            return false;
        }
        private static void EnableMousePointValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Slider slider)
            {
                if (e.NewValue is bool? == true)
                {
                    slider.MouseMove += TrackOnMouseMove;
                }
                else
                {
                    slider.MouseMove -= TrackOnMouseMove;
                }
            }
        }

        private static void TrackOnMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is Slider slider)
            {
                if (slider.Template.FindName("PART_Track", slider) is Track track)
                {
                    slider.SetValue(MousePointValueProperty, track.ValueFromPoint(e.GetPosition(track)));
                }
            }

        }
    }
}