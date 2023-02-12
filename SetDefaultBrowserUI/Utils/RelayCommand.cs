using System;
using System.Windows.Input;

namespace SetDefaultBrowserUI.Utils
{
    public class RelayCommand : ICommand
    {
        private readonly Predicate<object>? _canExecute;
        private readonly Action<object> _execute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action<object> execute, Predicate<object>? canExecute=null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
        
    }
}
