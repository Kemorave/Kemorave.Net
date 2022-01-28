using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
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
            obj.SetValue(RandomColorBrushProperty, PickBrush());

            return (Brush)obj.GetValue(RandomColorBrushProperty);

        }

        public static void SetRandomColorBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(RandomColorBrushProperty, value);
        }

        public static readonly DependencyProperty RandomColorBrushProperty =
           DependencyProperty.RegisterAttached("RandomColorBrush", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(PickBrush()), validateValueCallback);

        private static bool validateValueCallback(object value)
        {
            return true;
        }

        public static int random = 10;
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






















        #region Click/Key events


        public static InputBindingCollection GetInputBindings(DependencyObject obj)
        {
            return (InputBindingCollection)obj.GetValue(InputBindingsProperty);
        }

        public static void SetInputBindings(DependencyObject obj, InputBindingCollection value)
        {
            obj.SetValue(InputBindingsProperty, value);
        }

        // Using a DependencyProperty as the backing store for InputBindings.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputBindingsProperty =
            DependencyProperty.RegisterAttached("InputBindings", typeof(InputBindingCollection), typeof(ControlHelper), new PropertyMetadata(null, OnInputBindingCollectionChanged));

        private static void OnInputBindingCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            { 
                if (e.NewValue is InputBindingCollection collection && collection.Count > 0)
                { 
                    foreach (InputBinding item in collection)
                    { 
                        if(item.Command!=null&& item.Gesture!=null)
                        element.InputBindings.Add(new InputBinding(item.Command, item.Gesture) { CommandParameter =item.CommandParameter?? element.DataContext,CommandTarget = item.CommandTarget});
                    }
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
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

        public enum ClickEvent { Click, DoubleClick, TripleClick }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CommandClickProperty = DependencyProperty.RegisterAttached("CommandClick", typeof(ClickEvent?), typeof(ControlHelper), new UIPropertyMetadata(null, CommandClickPropertyChanged));
        public static void SetCommandClick(DependencyObject control, ClickEvent st)
        {
            control.SetValue(CommandClickProperty, st);
        }
        public static ClickEvent? GetCommandClick(DependencyObject control)
        {
            var val = control.GetValue(CommandClickProperty);
            if (val is ClickEvent clickEvent)
                return clickEvent;
            return null;
        }
        private static void CommandClickPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Debug.WriteLine(d + " Command ini");

            if (d is UIElement element)
            {
                element.PreviewMouseDown -= Element_MouseDown;
                if (e.NewValue is ClickEvent eventName)
                {
                    Debug.WriteLine($"Event click : {eventName} \nElement : {(element).GetType().Name}");
                    element.PreviewMouseDown += Element_MouseDown;
                }
            }

        }
        private static void Element_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (sender is FrameworkElement element)
            {
                if (GetCommand(element) is ICommand command)
                {
                    if (command.CanExecute(GetCommandParameter(element)))
                        if (GetCommandClick(element) is ClickEvent clickEvent)
                        {
                            switch (clickEvent)
                            {
                                case ClickEvent.Click:
                                    if (e.ClickCount == 1)
                                    {
                                        command.Execute(GetCommandParameter(element));
                                    }
                                    break;
                                case ClickEvent.DoubleClick:
                                    if (e.ClickCount == 2)
                                    {
                                        command.Execute(GetCommandParameter(element));
                                    }
                                    break;
                                case ClickEvent.TripleClick:
                                    if (e.ClickCount == 3)
                                    {
                                        command.Execute(GetCommandParameter(element));
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CommandKeyProperty = DependencyProperty.RegisterAttached("CommandKey", typeof(System.Windows.Input.Key?), typeof(ControlHelper), new UIPropertyMetadata(null, CommandKeyPropertyChanged));
        public static void SetCommandKey(DependencyObject control, Key? st)
        {
            control.SetValue(CommandKeyProperty, st);
        }
        public static Key? GetCommandKey(DependencyObject control)
        {
            var val = control.GetValue(CommandKeyProperty);
            if (val is Key key)
                return key;
            return null;
        }
        private static void CommandKeyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Debug.WriteLine(d + " Command ini");

            if (d is UIElement element)
            {
                element.PreviewKeyDown -= Element_PreviewKeyDown;
                if (e.NewValue is Key key)
                {
                    Debug.WriteLine($"Event key : {key} \nElement : {(element).GetType().Name}");

                    element.PreviewKeyDown += Element_PreviewKeyDown;
                }
            }
        }
        private static void Element_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is FrameworkElement element)

                if (e.Key.Equals(GetCommandKey(element)))
                {
                    if (GetCommand(element) is ICommand command)

                        if (command.CanExecute(GetCommandParameter(element)))
                        {
                            command.Execute(GetCommandParameter(element));
                        }
                }
        }



        #endregion





        #region Animations



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
        #endregion


        #region Visiblity
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
        public static bool GetIsHidden(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsHiddenProperty);
        }

        public static void SetIsHidden(DependencyObject obj, bool value)
        {
            obj.SetValue(IsHiddenProperty, value);
        }
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
        public static bool GetIsCollapsed(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCollapsedProperty);
        }

        public static void SetIsCollapsed(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCollapsedProperty, value);
        }

        public static bool GetIsNotHidden(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsNotHiddenProperty);
        }

        public static void SetIsNotHidden(DependencyObject obj, bool value)
        {
            obj.SetValue(IsNotHiddenProperty, value);
        }

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
        #endregion
    }
}