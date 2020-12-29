using System;
using System.Globalization;


namespace Kemorave.Components.ValueConverters
{
 public class ArrayToString : IValueConverter
 {
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
   try
   {
    string val = "";
    foreach (var item in (value as Array))
    {
     if (val == "")
      val += item;
     else
      val += "," + item;
    }
    return val;
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
