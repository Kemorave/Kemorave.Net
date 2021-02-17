using System;

namespace Kemorave
{
    public class ThreadingHelp
    {

        public static void Initialize(Kemorave.IMainUIThreadIInvoker invoker)
        {
            ThreadingHelp.MainUIThreadIInvoker = invoker;
        }
        public static Kemorave.IMainUIThreadIInvoker MainUIThreadIInvoker { get; set; }
        public static void RunOnMainUIThread(Action action)
        {
            if (MainUIThreadIInvoker != null)
            {
                MainUIThreadIInvoker.Invoke(action);
            }
            else
            {
                action.Invoke();
            }
        }
    }
}