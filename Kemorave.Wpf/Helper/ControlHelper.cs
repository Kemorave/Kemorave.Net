using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Kemorave.Wpf.Helper
{
    public class ControlHelper
    {

        public static void StartDoubleAnimation(UIElement uIElement, DependencyProperty dp, double toValue, Duration duration, HandoffBehavior handoffBehavior = HandoffBehavior.Compose)
        {
            if (double.IsNaN(toValue) || double.IsInfinity(toValue))
            {
                return;
            }
            uIElement.BeginAnimation(dp, new DoubleAnimation(toValue, duration, FillBehavior.HoldEnd), handoffBehavior);
        }




        public static Brush GetRandomColorBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(RandomColorBrushProperty);
        }

        public static void SetRandomColorBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(RandomColorBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for RandomColorBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RandomColorBrushProperty =
            DependencyProperty.RegisterAttached("RandomColorBrush", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(PickBrush()));

        public static Brush PickBrush()
        {
            Brush result;

            Random rnd = new Random();

            Type brushesType = typeof(Brushes);

            System.Reflection.PropertyInfo[] properties = brushesType.GetProperties();

            int random = rnd.Next(properties.Length);
            result = (Brush)properties[random].GetValue(null, null);

            return result;
        }

        public static bool GetIsHidden(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsHiddenProperty);
        }

        public static void SetIsHidden(DependencyObject obj, bool value)
        {
            obj.SetValue(IsHiddenProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsHidden.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsHiddenProperty =
            DependencyProperty.RegisterAttached("IsHidden", typeof(bool), typeof(ControlHelper), new PropertyMetadata(false, IsHiddenChanged));

        private static void IsHiddenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uIElement)
            {
                switch ((bool)e.NewValue)
                {
                    case true:
                        uIElement.Visibility = Visibility.Collapsed; break;
                    default:
                        uIElement.Visibility = Visibility.Visible; break;
                }
            }
        }
        public static bool GetIsCollapsed(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCollapsedProperty);
        }

        public static void SetIsCollapsed(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCollapsedProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsCollapsed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.RegisterAttached("IsCollapsed", typeof(bool), typeof(ControlHelper), new PropertyMetadata(false, IsCollapsedChanged));

        private static void IsCollapsedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uIElement)
            {
                switch ((bool)e.NewValue)
                {
                    case true:
                        uIElement.Visibility = Visibility.Collapsed; break;
                    default:
                        uIElement.Visibility = Visibility.Visible; break;
                }
            }
        }
    }
}