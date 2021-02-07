using System.Windows;
using System.Windows.Media;
namespace Kemorave.Wpf.Helper
{

 public static class RippleHelper
 {
  // Fields
  public static readonly DependencyProperty ClipToBoundsProperty = DependencyProperty.RegisterAttached("ClipToBounds", typeof(bool), typeof(RippleHelper), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));
  public static readonly DependencyProperty IsCenteredProperty = DependencyProperty.RegisterAttached("IsCentered", typeof(bool), typeof(RippleHelper), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
  public static readonly DependencyProperty IsDisabledProperty = DependencyProperty.RegisterAttached("IsDisabled", typeof(bool), typeof(RippleHelper), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
  public static readonly DependencyProperty RippleSizeMultiplierProperty = DependencyProperty.RegisterAttached("RippleSizeMultiplier", typeof(double), typeof(RippleHelper), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.Inherits));
  public static readonly DependencyProperty FeedbackProperty = DependencyProperty.RegisterAttached("Feedback", typeof(Brush), typeof(RippleHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));
  public static readonly DependencyProperty RippleOnTopProperty = DependencyProperty.RegisterAttached("RippleOnTop", typeof(bool), typeof(RippleHelper), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));

  // Methods
  public static bool GetClipToBounds(DependencyObject element) =>
      ((bool)element.GetValue(ClipToBoundsProperty));

  public static Brush GetFeedback(DependencyObject element) =>
      ((Brush)element.GetValue(FeedbackProperty));

  public static bool GetIsCentered(DependencyObject element) =>
      ((bool)element.GetValue(IsCenteredProperty));

  public static bool GetIsDisabled(DependencyObject element) =>
      ((bool)element.GetValue(IsDisabledProperty));

  public static bool GetRippleOnTop(DependencyObject element) =>
      ((bool)element.GetValue(RippleOnTopProperty));

  public static double GetRippleSizeMultiplier(DependencyObject element) =>
      ((double)element.GetValue(RippleSizeMultiplierProperty));

  public static void SetClipToBounds(DependencyObject element, bool value)
  {
   element.SetValue(ClipToBoundsProperty, value);
  }

  public static void SetFeedback(DependencyObject element, Brush value)
  {
   element.SetValue(FeedbackProperty, value);
  }

  public static void SetIsCentered(DependencyObject element, bool value)
  {
   element.SetValue(IsCenteredProperty, value);
  }

  public static void SetIsDisabled(DependencyObject element, bool value)
  {
   element.SetValue(IsDisabledProperty, value);
  }

  public static void SetRippleOnTop(DependencyObject element, bool value)
  {
   element.SetValue(RippleOnTopProperty, value);
  }

  public static void SetRippleSizeMultiplier(DependencyObject element, double value)
  {
   element.SetValue(RippleSizeMultiplierProperty, value);
  }
 }

}