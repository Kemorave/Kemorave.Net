using System;
using System.Drawing;
using System.Globalization;


namespace Kemorave.Components.ValueConverters
{
 public class PercentageToColor : IValueConverter
 {
  private  Color ColorValue = (Color.Gray);
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
   if (value is double)
   {
    if ((double)value <= 0)
    {
     return ColorValue;
    }
    if ((double)value > 0)
    {
     ColorValue = (Color.Blue);
    }
    if ((double)value >= 50)
    {
     ColorValue = (Color.Orange);
    }
    if ((double)value >= 70)
    {
     ColorValue = (Color.OrangeRed);
    }
    if ((double)value >= 80)
    {
     ColorValue = (Color.Red);
    }
   }
   if (value is int)
   {
    if ((int)value <= 0)
    {
     return ColorValue;
    }
    if ((int)value > 0)
    {
     ColorValue = (Color.Blue);
    }
    if ((int)value >= 50)
    {
     ColorValue = (Color.Orange);
    }
    if ((int)value >= 70)
    {
     ColorValue = (Color.OrangeRed);
    }
    if ((int)value >= 80)
    {
     ColorValue = (Color.Red);
    }
   }
   if (value is decimal)
   {
    if ((decimal)value <= 0)
    {
     return ColorValue;
    }
    if ((decimal)value > 0)
    {
     ColorValue = (Color.Blue);
    }
    if ((decimal)value >= 50)
    {
     ColorValue = (Color.Orange);
    }
    if ((decimal)value >= 70)
    {
     ColorValue = (Color.OrangeRed);
    }
    if ((decimal)value >= 80)
    {
     ColorValue = (Color.Red);
    }
   }
   if (value is float)
   {
    if ((float)value <= 0)
    {
     return ColorValue;
    }
    if ((float)value > 0)
    {
     ColorValue = (Color.Blue);
    }
    if ((float)value >= 50)
    {
     ColorValue = (Color.Orange);
    }
    if ((float)value >= 70)
    {
     ColorValue = (Color.OrangeRed);
    }
    if ((float)value >= 80)
    {
     ColorValue = (Color.Red);
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
     case "Blue": return 0;
     case "Orange": return 50;
     case "OrangeRed": return 70;
     case "Red": return 80;
     default:
      break;
    }
   }
   return value;
  }
 }
}