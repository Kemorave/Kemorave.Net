using System;
using System.Windows.Input;

namespace Kemorave.Command
{
 public sealed class ObjectCommand : RelayCommand<object>
 {
  public ObjectCommand(Action<object> execute) : base(execute)
  {
  }

  public ObjectCommand(Action<object> execute, Predicate<object> canExecute) : base(execute, canExecute)
  {
  }
 }
}