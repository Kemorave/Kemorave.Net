using System;
using System.Windows.Input;

namespace Kemorave.Command
{
    public class RelayCommand<T> : ICommand where T : class
    {
        public RelayCommand()
        {
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public RelayCommand(Action<T> execute) : this(execute, null)
        {
        }

        private readonly Predicate<T> _canExecute;

        private readonly Action<T> _execute;

        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }
            if (parameter == null)
            {
                return false;
            }
            return _canExecute(parameter as T);
        }

        public void Execute(object parameter)
        {
            if (parameter == null)
            {
                return;
            }
            _execute?.Invoke(parameter as T);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _TargetExecuteMethod;

        private readonly Func<bool> _TargetCanExecuteMethod;

        public Func<bool> TargetCanExecuteMethod => _TargetCanExecuteMethod;

        public RelayCommand()
        {
        }

        public RelayCommand(Action executeMethod)
        {
            _TargetExecuteMethod = executeMethod;
        }

        public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            _TargetExecuteMethod = executeMethod;
            _TargetCanExecuteMethod = canExecuteMethod;
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (TargetCanExecuteMethod != null)
            {
                return TargetCanExecuteMethod();
            }

            if (_TargetExecuteMethod != null)
            {
                return true;
            }

            return false;
        }

        // Beware - should use weak references if command instance lifetime is longer than lifetime
        // of UI objects that get hooked up to command

        // Prism commands solve this in their implementation
        public event EventHandler CanExecuteChanged;

       
        public virtual bool CanExecute()
        {
            if (_TargetCanExecuteMethod == null)
            {
                return true;
            } 
            return _TargetCanExecuteMethod();
        }

        public void Execute(object per)
        {
            if (_TargetCanExecuteMethod != null)
            {
				if (!_TargetCanExecuteMethod())
				{
                    return;
				}
            }

            _TargetExecuteMethod?.Invoke( );
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}