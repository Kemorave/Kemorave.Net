using System;
using System.Globalization;


namespace Kemorave.Components.ValueConverters
{
 public class EnumToArray : IValueConverter
 {
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
   try
   {
    return Enum.GetNames(value.GetType());
   }
   catch (Exception) { }
   return value;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
   return value;
  }
 }
}
