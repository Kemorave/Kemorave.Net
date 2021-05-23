using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Kemorave.Wpf.Helper
{

    public static class CustomCommands
    {
        static CustomCommands()
        {
            ToggleCommand = new RelayCommand<UIElement>(Toggle, CanToggle);
        }
        private static void Toggle(UIElement obj)
        {
            switch (obj)
            {
                case ToggleButton element: element.IsChecked = !element.IsChecked; break;
                case Popup element: element.IsOpen = !element.IsOpen; break;
                case MenuItem element: element.IsChecked = !element.IsChecked; break;
                case ComboBox element: element.IsDropDownOpen = !element.IsDropDownOpen; break;
                default:
                    break;
            }
        }

        private static bool CanToggle(UIElement arg)
        {
            return arg != null;
        }
        public static ICommand ToggleCommand { get; }
    }
     class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;

        private readonly Func<T, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (_canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute, bool keepTargetAlive = false)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            _execute = (execute);
            if (canExecute != null)
            {
                _canExecute = canExecute;
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            if (parameter == null && typeof(T).IsValueType)
            {
                return _canExecute(default(T));
            }
            if (parameter == null || parameter is T)
            {
                return _canExecute((T)parameter);
            }

            return false;
        }

        public virtual void Execute(object parameter)
        {
            object obj = parameter;
            if (parameter != null && parameter.GetType() != typeof(T) && parameter is IConvertible)
            {
                obj = Convert.ChangeType(parameter, typeof(T), null);
            }
            if (CanExecute(obj) && _execute != null)
            {
                if (obj == null)
                {
                    if (typeof(T).IsValueType)
                    {
                        _execute(default(T));
                    }
                    else
                    {
                        _execute((T)obj);
                    }
                }
                else
                {
                    _execute((T)obj);
                }
            }
        }
    }

}
