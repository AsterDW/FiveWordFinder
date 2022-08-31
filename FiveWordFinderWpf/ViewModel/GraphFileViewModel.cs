using FiveWordFinderWpf.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using FiveWordFinder.WordProcessing;
using FiveWordFinder.WordProcessing.Events;
using System.Threading;

namespace FiveWordFinderWpf.ViewModel
{
    public class GraphFileViewModel : ViewModelBase
    {
        private ApplicationState _applicationState;

        private string _fileName = string.Empty;
        private IGraphGenerator _generator;

        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }

        #region Progress Info
        private int _progressPercent;
        public int ProgressPercent
        {
            get
            {
                return _progressPercent;
            }
            set
            {
                _progressPercent = value;
                OnPropertyChanged();
            }
        }

        private string _progressMessage;
        public string ProgressMessage
        {
            get
            {
                return _progressMessage;
            }
            set
            {
                _progressMessage = value;
                OnPropertyChanged();
            }
        }
        #endregion Progress Info

        #region Timers
        private DispatcherTimer? _timer;
        private Stopwatch? _loadingStopwatch;
        private string _elapsedTimeString = "00:00.000";
        public string ElapsedTime 
        { 
            get { return _elapsedTimeString; } 
            set
            {
                _elapsedTimeString = value;
                OnPropertyChanged();
            }
        }

        #endregion Timers

        public ObservableCollection<WordDataView> WordsList { get; private set; }

        public int TotalNeighbors { get { return WordsList.Sum(w => w.NeighborsCount); } }

        private StatusStates _graphStatus = StatusStates.NotGenerated;
        private StatusStates GraphStatus 
        { 
            get { return _graphStatus; }
            set
            {
                _graphStatus = value;
                OnStatusChanged();
            }
        }
        private CancellationTokenSource? _cancellationTokenSource;

        public GraphFileViewModel(ApplicationState applicationState, IGraphGenerator graphGenerator)
        {
            _generator = graphGenerator;
            InitGenerator(_generator);
            
            _applicationState = applicationState;
            _progressMessage = String.Empty;
            WordsList = new ObservableCollection<WordDataView>();
            WordsList.CollectionChanged += (o, e) => { OnPropertyChanged(nameof(TotalNeighbors)); };

            _applicationState.PropertyChanged += _applicationState_PropertyChanged;
            GraphStatus = StatusStates.NotGenerated;
            RegisterValidators();
            FileName = GetFileName();
        }

        private void RegisterValidators()
        {
            RegisterPropertyValidator(nameof(FileName), () => {
                return new System.Windows.Controls.ValidationResult(_applicationState.HasValidWordsFilePath,
                    _applicationState.HasValidWordsFilePath ? string.Empty : "Invalid File");
            });
        }

        private void InitGenerator(IGraphGenerator generator)
        {
            
            generator.ProgressChanged += GraphGenerator_ProgressChanged;
            generator.WordAdded += (o, e) =>
            {
                _applicationState.MainThreadDispatcher.Invoke(() => 
                {
                    WordsList.Add(new WordDataView(e.Word));
                });
            };
        }

        protected override void OnIsActiveChanged()
        {
            CheckStartGenerating();
            base.OnIsActiveChanged();
        }

        protected void OnStatusChanged()
        {
            if (GraphStatus == StatusStates.NotGenerated)
            {
                _cancellationTokenSource?.Cancel();

                WordsList.Clear();
                ProgressPercent = 0;
                ProgressMessage = String.Empty;

                _timer?.Stop();
                _loadingStopwatch?.Reset();
                UpdateElapsedTimeString();
            }
        }

        private void CheckStartGenerating()
        {
            if (!IsActive)
                return;

            if (GraphStatus == StatusStates.Generating ||
                GraphStatus == StatusStates.Generated) return;

            if (_applicationState.HasValidWordsFilePath &&
                File.Exists(_applicationState.WordsFilePath))
            {
                GraphStatus = StatusStates.Generating;
                StartTimer();
                _cancellationTokenSource = new CancellationTokenSource();
                var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                _generator.GraphFromFileAsync(_applicationState.WordsFilePath,
                                              5,
                                              _cancellationTokenSource.Token).ContinueWith(t =>
                {
                    StopTimer();
                    GraphStatus = StatusStates.Generated;
                    if (t.Exception == null && !t.IsCanceled)
                    {
                        _applicationState.WordGraph = t.Result;
                        ProgressMessage = "Complete";
                    }
                    else
                    {
                        GraphStatus = StatusStates.NotGenerated;
                    }
                }, uiScheduler);
            }
        }

        private void _applicationState_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_applicationState.WordsFilePath))
            {
                GraphStatus = StatusStates.NotGenerated;
                FileName = GetFileName();
                CheckStartGenerating();
            }
        }

        private string GetFileName()
        {
            return _applicationState.HasValidWordsFilePath ?
                    Path.GetFileName(_applicationState.WordsFilePath) : "Invalid File Name";
        }

        private void StartTimer()
        {
            GraphStatus = StatusStates.Generating;

            if (_timer != null &&
                _timer.IsEnabled)
                return;

            _timer = new DispatcherTimer();
            _timer.Tick += (o, e) => { UpdateElapsedTimeString(); };
            _timer.Interval = TimeSpan.FromSeconds(1.0 / 10);
            _loadingStopwatch = Stopwatch.StartNew();
            _timer.Start();
        }

        private void StopTimer()
        {
            _loadingStopwatch?.Stop();
            _timer?.Stop();
            UpdateElapsedTimeString();
        }

        private void UpdateElapsedTimeString()
        {
            ElapsedTime = _loadingStopwatch?.Elapsed.ToString(@"mm\:ss\.fff") ?? "00:00.000";
        }

        private void GraphGenerator_ProgressChanged(object? sender, ProgressChangedMessageEventArgs e)
        {
            this.ProgressPercent = e.ProgressPercentage;

            this.ProgressMessage = e.ProgressMessage;
            
        }
    }
}
