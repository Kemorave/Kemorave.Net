using System;
using System.Globalization;


namespace Kemorave.Components.ValueConverters
{
 public class PercentValueToDigitValue : IValueConverter
 {
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
   if (value is double)
   {
    if ((double)value > 0)
    {
     return (((double)value * 1.0) / 100);
    }
   }
   if (value is int)
   {
    if ((int)value > 0)
    {
     return (((int)value * 1.0) / 100);
    }
   }

   if (value is float)
   {
    if ((float)value > 0)
    {
     return (((float)value * 1.0) / 100);
    }
   }

   return 0;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
   return value;
  }
 }
}