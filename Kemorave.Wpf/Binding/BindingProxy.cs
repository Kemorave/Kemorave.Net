using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemorave.Wpf.Binding
{
    public class BindingProxy : System.Windows.Freezable
    {
        #region Overrides of Freezable

        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        #endregion

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value);   }
        }

        public static readonly System.Windows.DependencyProperty DataProperty =
            System.Windows.DependencyProperty.Register("Data", typeof(object),
                                         typeof(BindingProxy));
    }
}
