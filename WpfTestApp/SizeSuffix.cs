using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WpfTestApp
{
    public class SizeSuffix : IValueConverter
    {
        Kemorave.Components.ValueConverters.ToSizeSuffix ToSizeSuffix = new Kemorave.Components.ValueConverters.ToSizeSuffix();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ToSizeSuffix.Convert(value, targetType,parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ToSizeSuffix.ConvertBack(value, targetType, parameter, culture);
        }
    }
}
