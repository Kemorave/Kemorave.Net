using System;
using System.Windows;

namespace Kemorave.Wpf.Helper
{
    public  class ProgressbarHelper
    {

        public static double GetAnimatedValue(DependencyObject obj)
        {
            return (double)obj.GetValue(AnimatedValueProperty);
        }

        public static void SetAnimatedValue(DependencyObject obj, double value)
        {
            obj.SetValue(AnimatedValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for AnimatedValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimatedValueProperty =
            DependencyProperty.RegisterAttached("AnimatedValue", typeof(double), typeof(ProgressbarHelper), new PropertyMetadata(0.0, AnimatedValueVhanged));

        private static void AnimatedValueVhanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           ControlHelper.StartDoubleAnimation((d as System.Windows.Controls.Primitives.RangeBase), System.Windows.Controls.Primitives.RangeBase.ValueProperty, (double)e.NewValue,GetAnimationDuration (d ));
        }



        public static Duration GetAnimationDuration(DependencyObject obj)
        {
            return (Duration)obj.GetValue(AnimationDurationProperty);
        }

        public static void SetAnimationDuration(DependencyObject obj, Duration value)
        {
            obj.SetValue(AnimationDurationProperty, value);
        }

        // Using a DependencyProperty as the backing store for AnimationDuration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.RegisterAttached("AnimationDuration", typeof(Duration), typeof(ProgressbarHelper), new PropertyMetadata(new Duration(TimeSpan.FromSeconds(0.6))));



    }
}