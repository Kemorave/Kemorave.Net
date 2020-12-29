using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kemorave
{
 public class ObservableObject : INotifyPropertyChanged
 {

  public event PropertyChangedEventHandler PropertyChanged;

  public virtual void RaisePropertyChanged([CallerMemberName] String propertyName = default)
  {
   PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
   if (propertyChanged != null)
   {
    propertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
   }
  }


 }
}
