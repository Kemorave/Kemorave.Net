using System;
using System.Threading.Tasks;
using System.Windows;

namespace WpfTestApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Kemorave.ThreadingHelper.Initialize(new MU());
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal class MU : Kemorave.IMainUIThreadIInvoker
    {
        public Task Invoke(Action action)
        {
            App.Current.Dispatcher.Invoke(action);
            return Task.CompletedTask;
        }
    }
}