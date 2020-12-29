using System;
using System.Collections.Generic;
using System.Text;

namespace Kemorave.Normalization
{
 public static class DataNormalization
 {
  /// <summary>
  /// "{0:n1} {1}"
  /// </summary>
  public const string SizeSuffixStringFormat = "{0:n1} {1}";
  /// <summary>
  ///  "Byte", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" 
  /// </summary>
  public static string[] SizeSuffixes = { "Byte", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
  public static string ToSizeSuffix(long? value)
  {
   if (value == null)
   {
    return string.Format(SizeSuffixStringFormat, 0, SizeSuffixes[0]);
   }
   if (value < 0) { return value.ToString(); }
   int i = 0;
   decimal dValue = (decimal)value;
   while (Math.Round(dValue / 1024) >= 1)
   {
    dValue /= 1024;
    i++;
   }
   return string.Format(SizeSuffixStringFormat, dValue, SizeSuffixes[i]);
  }

  public static decimal ToPercentage(long value, long maximume)
  {
   try
   {
    if (value != 0 && maximume != 0)
    {
     return Math.Round(System.Convert.ToDecimal((value * 100.0) / maximume), 2);
    }
   }
   catch (Exception) { }
   return 0m;
  }

 }
}