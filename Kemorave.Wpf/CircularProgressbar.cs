using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kemorave.Wpf
{
    [System.ComponentModel.DesignerCategory("Common")]
    [System.ComponentModel.DefaultProperty("Content")]
    [System.ComponentModel.DesignTimeVisible(true)]

    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Kemorave.Wpf"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Kemorave.Wpf;assembly=Kemorave.Wpf"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CircularProgressbar/>
    ///
    /// </summary>
    public class CircularProgressbar : System.Windows.Controls.Primitives.RangeBase
    {
        static CircularProgressbar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularProgressbar), new FrameworkPropertyMetadata(typeof(CircularProgressbar)));
        }


        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(CircularProgressbar), new PropertyMetadata(null));



        public double ArcThickness
        {
            get => (double)GetValue(ArcThicknessProperty);
            set => SetValue(ArcThicknessProperty, value);
        }

        // Using a DependencyProperty as the backing store for ArcThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArcThicknessProperty =
            DependencyProperty.Register("ArcThickness", typeof(double), typeof(CircularProgressbar), new PropertyMetadata(10.0));


        public double OriginRotationDegrees
        {
            get => (double)GetValue(OriginRotationDegreesProperty);
            set => SetValue(OriginRotationDegreesProperty, value);
        }

        // Using a DependencyProperty as the backing store for OriginRotationDegrees.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OriginRotationDegreesProperty =
            DependencyProperty.Register("OriginRotationDegrees", typeof(double), typeof(CircularProgressbar), new PropertyMetadata(90.0));


        public SweepDirection SweepDirection
        {
            get => (SweepDirection)GetValue(SweepDirectionProperty);
            set => SetValue(SweepDirectionProperty, value);
        }

        // Using a DependencyProperty as the backing store for SweepDirection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SweepDirectionProperty =
            DependencyProperty.Register("SweepDirection", typeof(SweepDirection), typeof(CircularProgressbar), new PropertyMetadata(default(SweepDirection)));


        public double ArcBorderThickness
        {
            get => (double)GetValue(ArcBorderThicknessProperty);
            set => SetValue(ArcBorderThicknessProperty, value);
        }

        // Using a DependencyProperty as the backing store for ArcBorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArcBorderThicknessProperty =
            DependencyProperty.Register("ArcBorderThickness", typeof(double), typeof(CircularProgressbar), new PropertyMetadata(3.0));





        public bool IsIndeterminate
        {
            get {  return (bool)GetValue(IsIndeterminateProperty); }
            set { SetValue(IsIndeterminateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsIndeterminate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsIndeterminateProperty =
            DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(CircularProgressbar), new PropertyMetadata(false));




    }
}
