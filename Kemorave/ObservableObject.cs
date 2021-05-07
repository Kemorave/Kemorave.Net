using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
