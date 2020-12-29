using System;
using System.Collections.Generic;
using System.Text;

namespace Kemorave
{
 public class Threading
 {
  public static Kemorave.IMainUIThreadIInvoker MainUIThreadIInvoker { get; set; }
  public static void RunOnMainUIThread(Action action)
  {
   if (MainUIThreadIInvoker != null)
    MainUIThreadIInvoker.Invoke(action);
   else
    action.Invoke();
  }
 }
}