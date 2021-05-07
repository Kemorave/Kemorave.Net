using System;

namespace Kemorave
{

    public interface IMainUIThreadIInvoker
 {
  void Invoke(Action action);
 }
}