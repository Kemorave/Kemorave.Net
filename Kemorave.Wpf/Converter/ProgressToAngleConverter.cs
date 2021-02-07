using System;

namespace Kemorave.Wpf.Converter
{
    public class ProgressToAngleConverter : System.Windows.Data.IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (values[0] is double)
                {
                    double progress = (double)values[0];
                    System.Windows.Controls.Primitives.RangeBase bar = values[1] as System.Windows.Controls.Primitives.RangeBase;

                    return 359.999 * (progress / (bar.Maximum - bar.Minimum));
                }
                if (values[0] is int)
                {

                    double progress = System.Convert.ToDouble((int)values[0]);
                    System.Windows.Controls.Primitives.RangeBase bar = values[1] as System.Windows.Controls.Primitives.RangeBase;

                    return 359.999 * (progress / (bar.Maximum - bar.Minimum));
                }
            }
            catch (Exception) { }
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return (object[])value;
        }
    }
}