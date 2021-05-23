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
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ripple), new FrameworkPropertyMetadata(typeof(Ripple)));
   EventManager.RegisterClassHandler(typeof(ContentControl), Mouse.PreviewMouseUpEvent, new MouseButtonEventHandler(MouseButtonEventHandler), true);
   EventManager.RegisterClassHandler(typeof(ContentControl), Mouse.MouseMoveEvent, new MouseEventHandler(MouseMoveEventHandler), true);
   EventManager.RegisterClassHandler(typeof(Popup), Mouse.PreviewMouseUpEvent, new MouseButtonEventHandler(MouseButtonEventHandler), true);
   EventManager.RegisterClassHandler(typeof(Popup), Mouse.MouseMoveEvent, new MouseEventHandler(MouseMoveEventHandler), true);
  }

  public Ripple()
  {
            SizeChanged += new SizeChangedEventHandler(this.OnSizeChanged);
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
   foreach (Ripple ripple in PressedInstances.ToList())
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
    if (Content is FrameworkElement content)
    {
     Point point = content.TransformToAncestor(this).Transform(new Point(0.0, 0.0));
     if (FlowDirection == FlowDirection.RightToLeft)
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
     this.RippleX = (ActualWidth / 2.0) - (this.RippleSize / 2.0);
     this.RippleY = (ActualHeight / 2.0) - (this.RippleSize / 2.0);
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
            if (RippleHelper.GetIsCentered(this) && (Content is FrameworkElement content))
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
       ((Brush)GetValue(FeedbackProperty));
   set =>
       SetValue(FeedbackProperty, value);
  }

  public double RippleSize
  {
   get =>
       ((double)GetValue(RippleSizeProperty));
   private set =>
       SetValue(RippleSizePropertyKey, value);
  }

  public double RippleX
  {
   get =>
       ((double)GetValue(RippleXProperty));
   private set =>
       SetValue(RippleXPropertyKey, value);
  }

  public double RippleY
  {
   get =>
       ((double)GetValue(RippleYProperty));
   private set =>
       SetValue(RippleYPropertyKey, value);
  }

  public bool RecognizesAccessKey
  {
   get =>
       ((bool)GetValue(RecognizesAccessKeyProperty));
   set =>
       SetValue(RecognizesAccessKeyProperty, value);
  }
 }

}