using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FiveWordFinderWpf.Commands
{
    public class RelayCommand : ICommand
    {
        #region Fields 
        readonly Action<object?> _execute;
        readonly Predicate<object?>? _canExecute;
        #endregion Fields

        public RelayCommand(Action<object?> execute) : this(execute, null) { }
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute; _canExecute = canExecute;
        }

        #region ICommand Members 
        [DebuggerStepThrough]
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void Execute(object? parameter) { _execute(parameter); }
        #endregion ICommand Members 

        public void PostCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
