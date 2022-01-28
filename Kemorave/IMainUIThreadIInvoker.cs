using System;
using System.Threading.Tasks;

namespace Kemorave
{

    public interface IMainUIThreadIInvoker
    {
        Task Invoke(Action action);
    }
}