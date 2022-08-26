using FiveWordFinderWpf.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FiveWordFinderWpf.Services
{
	public class NavigationService<T> : INavigationService<T> where T : class, IActivatable
	{
		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler? PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion INotifyPropertyChanged

		private IServiceProvider _serviceProvider;

		private List<Type> _itemList;
		private Dictionary<Type, int> _itemIndexDictionary;

		private T? _currentItem;

		public T? CurrentView
		{
			get
			{
				return _currentItem;
			}
			private set
			{
				if (_currentItem != value)
				{
					if (_currentItem != null)
						_currentItem.IsActive = false;
					_currentItem = value;
                    if (_currentItem != null)
                        _currentItem.IsActive = true;
                    OnPropertyChanged();
					OnPropertyChanged(nameof(CanNavigateBack));
					OnPropertyChanged(nameof(CanNavigateForward));
				}
			}
		}

		public bool CanNavigateBack
		{
			get { return CurrentView != null && _itemIndexDictionary[CurrentView.GetType()] > 0; }
		}

		public bool CanNavigateForward
		{
			get { return CurrentView != null && _itemIndexDictionary[CurrentView.GetType()] < _itemList.Count - 1; }
		}

		public ReadOnlyCollection<Type> NavigationViews
		{
			get { return _itemList.AsReadOnly(); }
		}

		public NavigationService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
			_itemList = new List<Type>();
			_itemIndexDictionary = new Dictionary<Type, int>();
		}

		public bool NavigateTo<TView>() where TView : T
		{
			var type = typeof(TView);
			if (_itemIndexDictionary.ContainsKey(type))
			{
				var item = (TView)_serviceProvider.GetRequiredService(type);
				CurrentView = item;
				return true;
			}

			return false;
		}

		public bool NavigateTo(Type viewType)
		{
			if (_itemIndexDictionary.ContainsKey(viewType))
			{
				var item = (T)_serviceProvider.GetRequiredService(viewType);
				CurrentView = item;
				return true;
			}

			return false;
		}

		public void NavigateBack()
		{
			if (CurrentView is null)
				return;

			var curIndex = _itemIndexDictionary[CurrentView.GetType()];
			if (curIndex > 0)
			{
				Type typeToShow = _itemList[curIndex - 1];
				CurrentView = (T)_serviceProvider.GetRequiredService(typeToShow);
			}
		}

		public void NavigateForward()
		{
			if (CurrentView is null)
				return;

			var curIndex = _itemIndexDictionary[CurrentView.GetType()];
			if (curIndex < _itemList.Count - 1)
			{
				Type typeToShow = _itemList[curIndex + 1];
				CurrentView = (T)_serviceProvider.GetRequiredService(typeToShow);
			}
		}

		public void AddView<TView>() where TView : T
		{
			int newIndex = _itemList.Count;
			_itemList.Add(typeof(TView));
			_itemIndexDictionary.Add(typeof(TView), newIndex);
		}
	}
}
