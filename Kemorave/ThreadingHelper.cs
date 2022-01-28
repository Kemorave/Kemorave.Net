using System;
using System.Threading.Tasks;

namespace Kemorave
{
    public class ThreadingHelper
    {

        public static void Initialize(Kemorave.IMainUIThreadIInvoker invoker)
        {
            ThreadingHelper.MainUIThreadIInvoker = invoker;
        }
        public static Kemorave.IMainUIThreadIInvoker MainUIThreadIInvoker { get; set; }
        public static async Task RunOnMainUIThread(Action action)
        {
            if (MainUIThreadIInvoker != null)
            {
            await    MainUIThreadIInvoker.Invoke(action);
            }
            else
            {
                action.Invoke();
            }
        }
    }
}