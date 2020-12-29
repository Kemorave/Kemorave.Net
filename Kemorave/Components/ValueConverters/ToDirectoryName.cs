
using System;
using System.Globalization;


namespace Kemorave.Components.ValueConverters
{
 public class ToDirectoryName : IValueConverter
 {
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
   if (value is string)
   {
    return System.IO.Path.GetFileName( System.IO.Path.GetDirectoryName((string)value));
   }
   return value;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {

   return value;
  }
 }
}
