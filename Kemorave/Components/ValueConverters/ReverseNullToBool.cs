using System;
using System.Globalization;


namespace Kemorave.Components.ValueConverters
{
 class ReverseNullToBool : IValueConverter
 {
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
   if (value is null)
   {
    return false;
   }
   return true;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
   if (value is false)
   {
    return null;
   }
   return true;
  }
 }
}