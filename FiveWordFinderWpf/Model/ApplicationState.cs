using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using FiveWordFinder.WordProcessing.Model;

namespace FiveWordFinderWpf.Model
{
    public class ApplicationState : INotifyPropertyChanged
    {
		private Dispatcher _dispatcher;
		public Dispatcher MainThreadDispatcher
		{
			get
			{
				return _dispatcher;
			}
			private set
			{
                _dispatcher = value;
				OnPropertyChanged();
			}
		}

		private string _wordsFilePath;
		public string WordsFilePath
		{
			get
			{
				return _wordsFilePath;
			}
			set
			{
				_wordsFilePath = value;
				HasValidWordsFilePath = CheckFilePathValid(WordsFilePath);
				this.WordGraph = null;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasValidWordsFilePath));
			}
		}

		private WordGraph? _wordGraph;
		public WordGraph? WordGraph
		{
			get
			{
				return _wordGraph;
			}
			set
			{
				_wordGraph = value;
				HasWordGraph = _wordGraph != null;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasWordGraph));
			}
		}

		public bool HasValidWordsFilePath { get; private set; }

		public bool HasWordGraph { get; private set; }

		public event PropertyChangedEventHandler? PropertyChanged;

		public ApplicationState()
		{
			_dispatcher = Dispatcher.CurrentDispatcher;
			_wordsFilePath = String.Empty;
		}

        protected void OnPropertyChanged([CallerMemberName]string propertyName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

		private bool CheckFilePathValid(string filePath)
		{
			return System.IO.File.Exists(filePath);
		}
    }
}
