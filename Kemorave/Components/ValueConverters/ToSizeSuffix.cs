
using System;
using System.Globalization;


namespace Kemorave.Components.ValueConverters
{
 public class ToSizeSuffix : IValueConverter
 {
  /// <summary>
  /// "{0:n1} {1}"
  /// </summary>
  public static string StringFormat = "{0:n1} {1}";
  /// <summary>
  ///  "Byte", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" 
  /// </summary>
  public static string[] SizeSuffixes = { "Byte", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
  public static string SizeSuffix(long? value)
  {
   if (value == null)
   {
    return string.Format(StringFormat, 0, SizeSuffixes[0]);
   }
   if (value < 0) { return value.ToString(); }
   int i = 0;
   decimal dValue = (decimal)value;
   while (Math.Round(dValue / 1024) >= 1)
   {
    dValue /= 1024;
    i++;
   }
   return string.Format(StringFormat, dValue, SizeSuffixes[i]);
  }

  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
   if (value is long l)
   {
    return SizeSuffix(l);
   }
   if (value is double d)
   {
    return SizeSuffix(System.Convert.ToInt64(d));
   }
   if (value is int i)
   {
    return SizeSuffix(System.Convert.ToInt64(i));
   }
   if (value is string s)
   {
    return SizeSuffix(Int64.Parse(s));
   }
   return null;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
   throw new NotImplementedException();
  }
 }
}