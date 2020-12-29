using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Kemorave.Components.ValueConverters
{
    public interface IValueConverter
    {
        //
        // Summary:
        //     Implement this method to convert value to targetType by using parameter and culture.
        //
        // Parameters:
        //   value:
        //     The value to convert.
        //
        //   targetType:
        //     The type to which to convert the value.
        //
        //   parameter:
        //     A parameter to use during the conversion.
        //
        //   culture:
        //     The culture to use during the conversion.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        //
        // Summary:
        //     Implement this method to convert value back from targetType by using parameter
        //     and culture.
        //
        // Parameters:
        //   value:
        //     The value to convert.
        //
        //   targetType:
        //     The type to which to convert the value.
        //
        //   parameter:
        //     A parameter to use during the conversion.
        //
        //   culture:
        //     The culture to use during the conversion.
        //
        // Returns:
        //     To be added.
        //
        // Remarks:
        //     To be added.
        object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }

}
