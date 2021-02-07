using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Kemorave.Wpf
{
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
    ///     <MyNamespace:RectangleChart/>
    ///
    /// </summary>
    public class RectangleChart : System.Windows.Controls.Grid
    {
        static RectangleChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RectangleChart), new FrameworkPropertyMetadata(typeof(RectangleChart)));
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            foreach (UIElement item in Children)
            {
                if (item is FrameworkElement frameworkElement)
                {
                    if (frameworkElement.IsMouseOver)
                    {
                        SelectedGraph = frameworkElement?.DataContext;
                    }
                }
            }
        }

      
        public Style ChartItemStyle
        {
            get { return (Style)GetValue(ChartItemStyleProperty); }
            set { SetValue(ChartItemStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChartItemStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChartItemStyleProperty =
            DependencyProperty.Register("ChartItemStyle", typeof(Style), typeof(RectangleChart), new PropertyMetadata(null));


        public object SelectedGraph
        {
            get => GetValue(SelectedGraphProperty);
            set => SetValue(SelectedGraphProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedGraph.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedGraphProperty =
            DependencyProperty.Register("SelectedGraph", typeof(object), typeof(RectangleChart), new PropertyMetadata(null));


        public IList ItemsSource
        {
            get => (IList)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList), typeof(RectangleChart), new PropertyMetadata(null, ItemsPropertyChanged));

        public static void ItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {

                if (e.NewValue is IList list)
                {
                    RectangleChart grid = (d as RectangleChart);
                    grid.Children.Clear();
                    if (e.NewValue is System.Collections.Specialized.INotifyCollectionChanged changingList)
                    {
                        changingList.CollectionChanged += (s, a) =>
                        {
                            grid.UpdateItems(list.Cast<IGraphItem>().ToList());
                        };
                    }
                    grid.UpdateItems(list.Cast<IGraphItem>().ToList());
                }
            }
            else
            {
                Grid grid = (d as System.Windows.Controls.Grid);
                grid?.Children?.Clear();
            }
        }

        public virtual void UpdateItems(IList<IGraphItem> Items)
        {
            if (Items?.Count <= 0)
            {
                return;
            }
            RectangleChart grid = this;
            grid.Children.Clear();
            grid.ColumnDefinitions.Clear();
            int count = 0;

            foreach (IGraphItem item in Items.OrderBy(j => j.Percentage))
            {
                Rectangle pb = new Rectangle()
                {
                    DataContext = item, Style = this.ChartItemStyle
                };
                grid.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition() { Width = new GridLength(item.Percentage, GridUnitType.Star) });
                System.Windows.Controls.Grid.SetColumn(pb, count);
                grid.Children.Add(pb);
                count++;
            }
        }

    }
    public class CircularChart : RectangleChart
    {
        static CircularChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CircularChart), new FrameworkPropertyMetadata(typeof(CircularChart)));
        }
        public CircularChart() { }



        public override void UpdateItems(IList<IGraphItem> Items)
        {
            if (Items.Count == 0)
            {
                return;
            }
            Items = Items.OrderBy(h => h.Percentage).ToList();
            Grid grid = this;
            grid.Children.Clear();

            double PreValue = 0;
            var tmp = new List<Arc>();
            foreach (var item in Items)
            {
                PreValue += item.Percentage;
                var pb = new Arc()
                {
                    StartAngle = 0
                    ,
                    EndAngle = 359.999 * PreValue / 100
                    ,
                    DataContext = item
                };
                tmp.Add(pb);
            }
            foreach (var item in tmp.OrderByDescending(j => (j.DataContext as IGraphItem).Percentage))
            {
                grid.Children.Add(item);
            }
        }

    }

}