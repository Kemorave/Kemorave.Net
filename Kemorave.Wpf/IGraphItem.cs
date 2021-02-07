using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kemorave.Wpf
{
    public interface IGraphItem
    {
        double Percentage { get; }
        string Name { get; }
        string Description { get; }
        Brush Brush { get; set; }
        object Tag { get; set; }
    }
}