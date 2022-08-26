using FiveWordFinderWpf.Commands;
using FiveWordFinderWpf.Model;
using FiveWordFinderWpf.Services;
using FiveWordFinderWpf.View;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FiveWordFinderWpf.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ApplicationState ApplicationState { get; }

        public INavigationService<ViewModelBase> NavigationService { get; }

        public NavigateToCommand NavigateToCommand { get; private set; }

        public RelayCommand NavigateBackCommand { get; private set; }
        public RelayCommand NavigateNextCommand { get; private set; }

        public bool HomeChecked
        {
            get { return NavigationService.CurrentView is HomeViewModel; }
        }
        public bool GraphChecked
        {
            get { return NavigationService.CurrentView is GraphFileViewModel; }
        }
        public bool EvaluateChecked
        {
            get { return NavigationService.CurrentView is EvaluateGraphViewModel; }
        }

        public MainWindowViewModel(ApplicationState applicationState, INavigationService<ViewModelBase> navigationService)
        {
            NavigationService = navigationService;
            ApplicationState = applicationState;

            this.NavigateToCommand = new NavigateToCommand(NavigationService);
            this.NavigateBackCommand = new RelayCommand((o) => { NavigationService.NavigateBack(); }, (o) => { return NavigationService.CanNavigateBack; });
            this.NavigateNextCommand = new RelayCommand((o) => { NavigationService.NavigateForward(); }, (o) => { return NavigationService.CanNavigateForward; });

            InitNavigation();
        }

        private void InitNavigation()
        {
            NavigationService.AddView<HomeViewModel>();
            NavigationService.AddView<GraphFileViewModel>();
            NavigationService.AddView<EvaluateGraphViewModel>();

            NavigationService.PropertyChanged += NavigationService_PropertyChanged;
            NavigationService.NavigateTo<HomeViewModel>();
        }

        private void NavigationService_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HomeChecked));
            OnPropertyChanged(nameof(GraphChecked));
            OnPropertyChanged(nameof(EvaluateChecked));

            NavigateBackCommand.PostCanExecuteChanged();
            NavigateNextCommand.PostCanExecuteChanged();
        }
    }
}
