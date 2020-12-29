using System;
using System.Globalization;


namespace Kemorave.Components.ValueConverters
{
 public class PathToName : IValueConverter
 {
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
   if (value is string val)
   {
    return System.IO.Path.GetFileName(val);
   }
   return value;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
   if (value is object[])
   {
    if (((object[])value)[0] is string && ((object[])value)[1] is string)
    {
     return System.IO.Path.Combine((string)((object[])value)[0], (string)((object[])value)[1]);
    }
   }
   return value;
  }
 }
}
