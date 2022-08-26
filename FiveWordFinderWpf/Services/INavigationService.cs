using FiveWordFinderWpf.ViewModel;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;

namespace FiveWordFinderWpf.Services
{
    public interface INavigationService<T> : INotifyPropertyChanged where T : class
    {
        bool CanNavigateBack { get; }
        bool CanNavigateForward { get; }
        T? CurrentView { get; }
        ReadOnlyCollection<Type> NavigationViews { get; }

        void AddView<TView>() where TView : T;
        void NavigateBack();
        bool NavigateTo<TView>() where TView : T;
        bool NavigateTo(Type viewType);
        void NavigateForward();
    }
}