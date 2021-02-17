using Kemorave.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace WpfTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class Item : Kemorave.Wpf.IGraphItem
        {
            public Item(double percentage, string name, string description, Brush brush, object tag=null)
            {
                Percentage = percentage;
                Name = name ?? throw new ArgumentNullException(nameof(name));
                Description = description ?? throw new ArgumentNullException(nameof(description));
                Brush = brush ?? throw new ArgumentNullException(nameof(brush));
                Tag = tag;
            }

            public double Percentage { get; }

            public string Name { get; }

            public string Description { get; }

            public Brush Brush { get; set ; }
            public object Tag { get ; set ; }
        }
        public MainWindow()
        {
            InitializeComponent();
            //RecChart.UpdateItems(new List<IGraphItem>
            //{
            //    new Item(60,"Me","Out of low",Kemorave.Wpf.Helper.ControlHelper.PickBrush()),
            //    new Item(50,"Me","Out of low",Kemorave.Wpf.Helper.ControlHelper.PickBrush()),
            //    new Item(10,"Me","Out of low",Kemorave.Wpf.Helper.ControlHelper.PickBrush()),
            //    new Item(20,"Me","Out of low",Kemorave.Wpf.Helper.ControlHelper.PickBrush())

            //});
        }
    }
}
