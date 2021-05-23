using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;

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
            obj.SetValue(RandomColorBrushProperty, PickBrush());

            return (Brush)obj.GetValue(RandomColorBrushProperty);
            
        }

        public static void SetRandomColorBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(RandomColorBrushProperty, value);
        }

        // Using a DependencyProperty as the backing store for RandomColorBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RandomColorBrushProperty =
            DependencyProperty.RegisterAttached("RandomColorBrush", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(PickBrush()),validateValueCallback);

        private static bool validateValueCallback(object value)
        {
            return true;
        }

        public static int random=10;
        public static Brush PickBrush()
        {
            Brush result;
            Random rnd = new Random(random); 
            Type brushesType = typeof(Brushes);

            System.Reflection.PropertyInfo[] properties = brushesType.GetProperties();
           
             random = rnd.Next(properties.Length);
            result = (Brush)properties.ToList().ElementAtOrDefault(random).GetValue(null, null);
            Debug.WriteLine($"Random shit at {random}");
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




  


         
       

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(ControlHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static void SetCommandParameter(DependencyObject control, object st)
        {
            control.SetValue(CommandParameterProperty, st);
        }
        public static object GetCommandParameter(DependencyObject control)
        {
            var val = control.GetValue(CommandParameterProperty);
            if (val is object)
                return (object)val;
            return null;
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(ControlHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static void SetCommand(DependencyObject control, ICommand st)
        {
            control.SetValue(CommandProperty, st);
        }
        public static ICommand GetCommand(DependencyObject control)
        {
            var val = control.GetValue(CommandProperty);
            if (val is ICommand)
                return (ICommand)val;
            return null;
        }

        public static readonly DependencyProperty CommandEventNameProperty = DependencyProperty.RegisterAttached("CommandEventName", typeof(string), typeof(ControlHelper), new UIPropertyMetadata(null, CommandEventNamePropertyChanged));

        private static void CommandEventNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Debug.WriteLine(d + " Command ini");

            if (e.NewValue is string eventName && d is UIElement element)
            {
                Debug.WriteLine($"Event name : {eventName} \nElement : {(element).GetType().Name}");

                switch (eventName)
                {
                    case "Click": element.MouseDown += Element_MouseDown; break;
                    case "MouseDoubleClick": element.PreviewMouseLeftButtonDown += Element_MouseDown; break;
                    default:
                        break;
                }
            }
        }

        private static void Element_MouseDown(object sender, MouseButtonEventArgs e)
        {
           // Debug.WriteLine(sender + " Command ex");
            if (sender is FrameworkElement element)
            {
                if (GetCommand(element) is ICommand command)
                {
                    if (GetCommandEventName(element) == "Click" && e.ClickCount == 1)
                    {
                        if (command.CanExecute(GetCommandParameter(element)))
                        {
                            command.Execute(GetCommandParameter(element));
                        }
                    }
                    if (GetCommandEventName(element) == "MouseDoubleClick" && e.ClickCount == 2)
                    {
                        if (command.CanExecute(GetCommandParameter(element)))
                        {
                            command.Execute(GetCommandParameter(element));
                        }
                    }
                }
            }
        }

        public static void SetCommandEventName(DependencyObject control, string st)
        {
            control.SetValue(CommandEventNameProperty, st);
        }
        public static string GetCommandEventName(DependencyObject control)
        {
            var val = control.GetValue(CommandEventNameProperty);
            if (val is string)
                return (string)val;
            return null;
        }










        public static readonly DependencyProperty LoadedAnimationProperty = DependencyProperty.RegisterAttached("LoadedAnimation", typeof(Storyboard), typeof(ControlHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, LoadedAnimationProprtyChanged));
        private static void LoadedAnimationProprtyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement)
            {

                Storyboard st = GetLoadedAnimation(d);
                if (st != null)
                {

                    (d as FrameworkElement).Loaded += (sm, ar) =>
                    {
                        st.Begin((d as FrameworkElement));
                    };
                }

            }
        }
        public static void SetLoadedAnimation(DependencyObject control, Storyboard st)
        {
            control.SetValue(LoadedAnimationProperty, st);
        }
        public static Storyboard GetLoadedAnimation(DependencyObject control)
        {
            var val = control.GetValue(LoadedAnimationProperty);
            if (val is Storyboard)
                return (Storyboard)val;
            return null;
        }




        public static bool GetIsNotHidden(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsNotHiddenProperty);
        }

        public static void SetIsNotHidden(DependencyObject obj, bool value)
        {
            obj.SetValue(IsNotHiddenProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsNotHidden.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsNotHiddenProperty =
            DependencyProperty.RegisterAttached("IsNotHidden", typeof(bool), typeof(ControlHelper), new PropertyMetadata(true, IsNotHiddenChanged));

        private static void IsNotHiddenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uIElement)
            {
                switch ((bool)e.NewValue)
                {
                    case true:
                        uIElement.Visibility = Visibility.Visible; break;
                    default:
                        uIElement.Visibility = Visibility.Hidden; break;
                }
            }
        }
         ///////////////////////////////////////////////////////////////////////////////////////
       public static bool GetIsNotCollapsed(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsNotCollapsedProperty);
        }

        public static void SetIsNotCollapsed(DependencyObject obj, bool value)
        {
            obj.SetValue(IsNotCollapsedProperty, value);
        }

        public static readonly DependencyProperty IsNotCollapsedProperty =
            DependencyProperty.RegisterAttached("IsNotCollapsed", typeof(bool), typeof(ControlHelper), new PropertyMetadata(true, IsNotCollapsedChanged));
        private static void IsNotCollapsedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uIElement)
            {
                switch ((bool)e.NewValue)
                {
                    case true:
                        uIElement.Visibility = Visibility.Visible; break;
                    default:
                        uIElement.Visibility = Visibility.Collapsed; break;
                }
            }
            if (d is System.Windows.Controls.DataGridColumn col)
            {
                switch ((bool)e.NewValue)
                {
                    case true:
                        col.Visibility = Visibility.Visible; break;
                    default:
                        col.Visibility = Visibility.Collapsed; break;
                }
            }
        }
    }
}