using System;
using System.Globalization;


namespace Kemorave.Components.ValueConverters
{
 class NullToBool : IValueConverter
 {
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
   if (value is null)
   {
    return true;
   }
   return false;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
   if (value is true)
   {
    return null;
   }
   return false;
  }
 }
}