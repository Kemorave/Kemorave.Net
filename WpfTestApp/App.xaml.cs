using System;
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
            Kemorave.ThreadingHelp.Initialize(new MU());
        }
    }
    /// <summary>
    /// 
    /// </summary>
    internal class MU : Kemorave.IMainUIThreadIInvoker
    {
        public void Invoke(Action action)
        {
            App.Current.Dispatcher.Invoke(action);
        }
    }
}