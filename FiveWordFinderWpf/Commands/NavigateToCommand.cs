using FiveWordFinderWpf.Services;
using FiveWordFinderWpf.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FiveWordFinderWpf.Commands
{
    public class NavigateToCommand : ICommand
    {
        private INavigationService<ViewModelBase> _navigationService;

        public event EventHandler? CanExecuteChanged;

        private Func<bool> _canExecuteFunc;

        public NavigateToCommand(INavigationService<ViewModelBase> navigationService)
        {
            _navigationService = navigationService;
            _canExecuteFunc = () => { return true; };
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecuteFunc() ;
        }

        public void Execute(object? parameter)
        {
            if (parameter == null) return;

            if (parameter is Type)
            {
                _navigationService.NavigateTo((Type)parameter);
            }
        }

        public void AssignCanExecuteFunction(Func<bool> func)
        {
            if (func != null)
            {
                _canExecuteFunc = func;
            }
        }

        public void PostCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
