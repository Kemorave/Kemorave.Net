using Kemorave.Wpf.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Kemorave.Wpf
{
 [TemplateVisualState(GroupName = "CommonStates", Name = "Normal"), TemplateVisualState(GroupName = "CommonStates", Name = "MousePressed"), TemplateVisualState(GroupName = "CommonStates", Name = "MouseOut")]
 public class Ripple : ContentControl
 {
  // Fields
  public const string TemplateStateNormal = "Normal";
  public const string TemplateStateMousePressed = "MousePressed";
  public const string TemplateStateMouseOut = "MouseOut";
  private static readonly HashSet<Ripple> PressedInstances = new HashSet<Ripple>();
  public static readonly DependencyProperty FeedbackProperty = DependencyProperty.Register("Feedback", typeof(Brush), typeof(Ripple), new PropertyMetadata(null));
  private static readonly DependencyPropertyKey RippleSizePropertyKey = DependencyProperty.RegisterReadOnly("RippleSize", typeof(double), typeof(Ripple), new PropertyMetadata(0.0));
  public static readonly DependencyProperty RippleSizeProperty = RippleSizePropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey RippleXPropertyKey = DependencyProperty.RegisterReadOnly("RippleX", typeof(double), typeof(Ripple), new PropertyMetadata(0.0));
  public static readonly DependencyProperty RippleXProperty = RippleXPropertyKey.DependencyProperty;
  private static readonly DependencyPropertyKey RippleYPropertyKey = DependencyProperty.RegisterReadOnly("RippleY", typeof(double), typeof(Ripple), new PropertyMetadata(0.0));
  public static readonly DependencyProperty RippleYProperty = RippleYPropertyKey.DependencyProperty;
  public static readonly DependencyProperty RecognizesAccessKeyProperty = DependencyProperty.Register("RecognizesAccessKey", typeof(bool), typeof(Ripple), new PropertyMetadata(false));

  // Methods
  static Ripple()
  {
   FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(Ripple), new FrameworkPropertyMetadata(typeof(Ripple)));
   EventManager.RegisterClassHandler(typeof(ContentControl), Mouse.PreviewMouseUpEvent, new MouseButtonEventHandler(Ripple.MouseButtonEventHandler), true);
   EventManager.RegisterClassHandler(typeof(ContentControl), Mouse.MouseMoveEvent, new MouseEventHandler(Ripple.MouseMoveEventHandler), true);
   EventManager.RegisterClassHandler(typeof(Popup), Mouse.PreviewMouseUpEvent, new MouseButtonEventHandler(Ripple.MouseButtonEventHandler), true);
   EventManager.RegisterClassHandler(typeof(Popup), Mouse.MouseMoveEvent, new MouseEventHandler(Ripple.MouseMoveEventHandler), true);
  }

  public Ripple()
  {
   base.SizeChanged += new SizeChangedEventHandler(this.OnSizeChanged);
  }

  private static void MouseButtonEventHandler(object sender, MouseButtonEventArgs e)
  {
   foreach (Ripple ripple in PressedInstances)
   {
    if (ripple.Template.FindName("ScaleTransform", ripple) is ScaleTransform transform)
    {
     double scaleX = transform.ScaleX;
     TimeSpan timeSpan = TimeSpan.FromMilliseconds(300.0 * (1.0 - scaleX));
     if (ripple.Template.FindName("MousePressedToNormalScaleXKeyFrame", ripple) is EasingDoubleKeyFrame frame)
     {
      frame.KeyTime = KeyTime.FromTimeSpan(timeSpan);
     }
     if (ripple.Template.FindName("MousePressedToNormalScaleYKeyFrame", ripple) is EasingDoubleKeyFrame frame2)
     {
      frame2.KeyTime = KeyTime.FromTimeSpan(timeSpan);
     }
    }
    VisualStateManager.GoToState(ripple, "Normal", true);
   }
   PressedInstances.Clear();
  }

  private static void MouseMoveEventHandler(object sender, MouseEventArgs e)
  {
   foreach (Ripple ripple in PressedInstances.ToList<Ripple>())
   {
    Point position = Mouse.GetPosition(ripple);
    if (((position.X < 0.0) || (position.Y < 0.0)) || ((position.X >= ripple.ActualWidth) || (position.Y >= ripple.ActualHeight)))
    {
     VisualStateManager.GoToState(ripple, "MouseOut", true);
     PressedInstances.Remove(ripple);
    }
   }
  }

  public override void OnApplyTemplate()
  {
   base.OnApplyTemplate();
   VisualStateManager.GoToState(this, "Normal", false);
  }

  protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
  {
   if (RippleHelper.GetIsCentered(this))
   {
    if (base.Content is FrameworkElement content)
    {
     Point point = content.TransformToAncestor(this).Transform(new Point(0.0, 0.0));
     if (base.FlowDirection == FlowDirection.RightToLeft)
     {
      this.RippleX = (point.X - (content.ActualWidth / 2.0)) - (this.RippleSize / 2.0);
     }
     else
     {
      this.RippleX = (point.X + (content.ActualWidth / 2.0)) - (this.RippleSize / 2.0);
     }
     this.RippleY = (point.Y + (content.ActualHeight / 2.0)) - (this.RippleSize / 2.0);
    }
    else
    {
     this.RippleX = (base.ActualWidth / 2.0) - (this.RippleSize / 2.0);
     this.RippleY = (base.ActualHeight / 2.0) - (this.RippleSize / 2.0);
    }
   }
   else
   {
    Point position = e.GetPosition(this);
    this.RippleX = position.X - (this.RippleSize / 2.0);
    this.RippleY = position.Y - (this.RippleSize / 2.0);
   }
   if (!RippleHelper.GetIsDisabled(this))
   {
    VisualStateManager.GoToState(this, "Normal", false);
    VisualStateManager.GoToState(this, "MousePressed", true);
    PressedInstances.Add(this);
   }
   base.OnPreviewMouseLeftButtonDown(e);
  }

  private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
  {
   double actualWidth;
   double actualHeight;
   FrameworkElement content = base.Content as FrameworkElement;
   if (RippleHelper.GetIsCentered(this) && (content != null))
   {
    actualWidth = content.ActualWidth;
    actualHeight = content.ActualHeight;
   }
   else
   {
    actualWidth = sizeChangedEventArgs.NewSize.Width;
    actualHeight = sizeChangedEventArgs.NewSize.Height;
   }
   double num3 = Math.Sqrt(Math.Pow(actualWidth, 2.0) + Math.Pow(actualHeight, 2.0));
   this.RippleSize = (2.0 * num3) * RippleHelper.GetRippleSizeMultiplier(this);
  }

  // Properties
  public Brush Feedback
  {
   get =>
       ((Brush)base.GetValue(FeedbackProperty));
   set =>
       base.SetValue(FeedbackProperty, value);
  }

  public double RippleSize
  {
   get =>
       ((double)base.GetValue(RippleSizeProperty));
   private set =>
       base.SetValue(RippleSizePropertyKey, value);
  }

  public double RippleX
  {
   get =>
       ((double)base.GetValue(RippleXProperty));
   private set =>
       base.SetValue(RippleXPropertyKey, value);
  }

  public double RippleY
  {
   get =>
       ((double)base.GetValue(RippleYProperty));
   private set =>
       base.SetValue(RippleYPropertyKey, value);
  }

  public bool RecognizesAccessKey
  {
   get =>
       ((bool)base.GetValue(RecognizesAccessKeyProperty));
   set =>
       base.SetValue(RecognizesAccessKeyProperty, value);
  }
 }

}