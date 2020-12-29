using System;
using System.Drawing;
using System.Globalization;


namespace Kemorave.Components.ValueConverters
{
 public class ReversePercentageToColor : IValueConverter
 {
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
   var ColorValue = (Color.Gray);
   if (value is double)
   {
    if ((double)value < 0)
    {
     return ColorValue;
    }
    if ((double)value > 0)
    {
     ColorValue = (Color.Red);
    }
    if ((double)value >= 30)
    {
     ColorValue = (Color.OrangeRed);
    }
    if ((double)value >= 70)
    {
     ColorValue = (Color.Orange);
    }
    if ((double)value >= 80)
    {
     ColorValue = (Color.Blue);
    }
   }
   if (value is int)
   {
    if ((int)value < 0)
    {
     return ColorValue;
    }
    if ((int)value > 0)
    {
     ColorValue = (Color.Red);
    }
    if ((int)value >= 30)
    {
     ColorValue = (Color.OrangeRed);
    }
    if ((int)value >= 70)
    {
     ColorValue = (Color.Orange);
    }
    if ((int)value >= 80)
    {
     ColorValue = (Color.Blue);
    }
   }
   if (value is decimal)
   {
    if ((decimal)value < 0)
    {
     return ColorValue;
    }
    if ((decimal)value > 0)
    {
     ColorValue = (Color.Red);
    }
    if ((decimal)value >= 30)
    {
     ColorValue = (Color.OrangeRed);
    }
    if ((decimal)value >= 70)
    {
     ColorValue = (Color.Orange);
    }
    if ((decimal)value >= 80)
    {
     ColorValue = (Color.Blue);
    }
   }
   if (value is float)
   {
    if ((float)value < 0)
    {
     return ColorValue;
    }
    if ((float)value > 0)
    {
     ColorValue = (Color.Red);
    }
    if ((float)value >= 30)
    {
     ColorValue = (Color.OrangeRed);
    }
    if ((float)value >= 70)
    {
     ColorValue = (Color.Orange);
    }
    if ((float)value >= 80)
    {
     ColorValue = (Color.Blue);
    }
   }
   return ColorValue;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
   if (value is Color)
   {
    switch (((Color)value).ToString())
    {
     case "Blue": return 80;
     case "Orange": return 70;
     case "OrangeRed": return 30;
     case "Red": return 0;
     default:
      break;
    }
   }
   return value;
  }
 }
}