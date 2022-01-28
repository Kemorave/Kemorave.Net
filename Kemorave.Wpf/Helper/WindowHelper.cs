using System;
using System.Windows;

namespace Kemorave.Wpf.Helper
{
 public class WindowHelper
 {
  public static readonly DependencyProperty MovableProperty = DependencyProperty.RegisterAttached("Movable", typeof(bool), typeof(WindowHelper), new FrameworkPropertyMetadata(false , MovableProprtyChanged));
  private static void MovableProprtyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
   if (d is Window window)
   {

    if (GetMovable(d))
    {
     window.MouseDown += Window_MouseDown;
    }
    else
    {
     window.MouseDown -= Window_MouseDown;
    }

   }
  }

  private static void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
  {
   try
   {
    (sender as Window)?.DragMove();
   }
   catch (Exception)
   {
   }
  }

  public static void SetMovable(DependencyObject control, bool st)
  {
   control.SetValue(MovableProperty, st);
  }
  public static bool GetMovable(DependencyObject control)
  {
   var val = control.GetValue(MovableProperty);
   if (val is bool)
    return (bool)val;
   return false;
  }

  /// <summary>
  /// ////////////////////////////////////////////////////////
  /// </summary>

  public static readonly DependencyProperty InfoProgressBarValueProperty = DependencyProperty.RegisterAttached("InfoProgressBarValue", typeof(double), typeof(WindowHelper), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure, InfoProgressBarValueProprtyChanged));
  private static void InfoProgressBarValueProprtyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
   if (d is Window window)
   {
    var pr = GetInfoProgressBarValue(d);
    window.TaskbarItemInfo.ProgressValue = pr;

   }
  }
  public static void SetInfoProgressBarValue(DependencyObject control, double st)
  {
   control.SetValue(InfoProgressBarValueProperty, st);
  }
  public static double GetInfoProgressBarValue(DependencyObject control)
  {
   var val = control.GetValue(InfoProgressBarValueProperty);
   if (val is double)
    return (double)val;
   return 0;
  }

  /// <summary>
  /// ////////////////////////////////////////////////////////
  /// </summary>

  public static readonly DependencyProperty InfoProgressStateProperty = DependencyProperty.RegisterAttached("InfoProgressState", typeof(System.Windows.Shell.TaskbarItemProgressState), typeof(WindowHelper), new FrameworkPropertyMetadata(System.Windows.Shell.TaskbarItemProgressState.None, FrameworkPropertyMetadataOptions.AffectsMeasure, InfoProgressStateProprtyChanged));
  private static void InfoProgressStateProprtyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
   if (d is Window window)
   {
    var pr = GetInfoProgressState(d);
    window.TaskbarItemInfo.ProgressState = pr;
   }
  }
  public static void SetInfoProgressState(DependencyObject control, System.Windows.Shell.TaskbarItemProgressState st)
  {
   control.SetValue(InfoProgressStateProperty, st);
  }
  public static System.Windows.Shell.TaskbarItemProgressState GetInfoProgressState(DependencyObject control)
  {
   var val = control.GetValue(InfoProgressStateProperty);
   if (val is System.Windows.Shell.TaskbarItemProgressState)
    return (System.Windows.Shell.TaskbarItemProgressState)val;
   return System.Windows.Shell.TaskbarItemProgressState.None;
  }



        //////////////////////////////////////////
        


   
    }
}