using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using FiveWordFinderWpf.Model;
using FiveWordFinder.Extensions;
using FiveWordFinder.WordProcessing.Strategies;
using FiveWordFinderWpf.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using FiveWordFinder.WordProcessing.Model;
using System.IO;
using System.Threading;

namespace FiveWordFinderWpf.ViewModel
{
    internal class EvaluateGraphViewModel : ViewModelBase
    {
        private ApplicationState _applicationState;

        private StatusStates _status = StatusStates.NotGenerated;
        public StatusStates Status 
        {
            get { return _status; }
            private set
            {
                _status = value;
                OnStatusChanged();
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProgressMessage));
            }
        }

        private CancellationTokenSource? _cancellationTokenSource;

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

        private int? _currentStep;
        public int? CurrentStep
        {
            get
            {
                return _currentStep;
            }
            set
            {
                if (_currentStep != value)
                {
                    _currentStep = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ProgressMessage));
                }
            }
        }

        private int? _totalSteps;
        public int? TotalSteps
        {
            get
            {
                return _totalSteps;
            }
            set
            {
                if (_totalSteps != value)
                {
                    _totalSteps = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ProgressMessage));
                }
            }
        }

        public string ProgressMessage
        {
            get
            {
                var message = string.Empty;
                switch(Status)
                {
                    case StatusStates.Generated:
                        message = "Complete";
                        break;
                    case StatusStates.Generating:
                        message = string.Format("Evaluating Graph: {0} / {1}",
                                                CurrentStep,
                                                TotalSteps);
                        break;
                    default:
                        message = "Generate a Graph From a Words File First";
                        break;
                }
                    return message;
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

        private IGraphEvaluationNotify _graphEvaluator;

        public RelayCommand SaveCommand { get; private set; }

        public ObservableCollection<CliqueDataView> CliqueList { get; private set; }

        public EvaluateGraphViewModel(ApplicationState applicationState, IGraphEvaluationNotify graphEvaluator)
        {
            _applicationState = applicationState;
            _graphEvaluator = graphEvaluator;
            InitEvaluator();
            SaveCommand = InitSaveCommand();

            CliqueList = new ObservableCollection<CliqueDataView>();

            _applicationState.PropertyChanged += _applicationState_PropertyChanged;
            Status = StatusStates.NotGenerated;
        }

        private void _applicationState_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_applicationState.WordGraph))
            {
                Status = StatusStates.NotGenerated;

                CheckStartGenerating();
            }
        }

        private RelayCommand InitSaveCommand()
        {
            var saveCommand = new RelayCommand((o) =>
            {
                var sd = new SaveFileDialog();
                sd.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                sd.DefaultExt = "txt";
                sd.OverwritePrompt = true;
                if (sd.ShowDialog() == true)
                {
                    try
                    {
                        WriteCliques(sd.FileName);
                        MessageBox.Show("Export Complete.");
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            },
            (o) =>
            {
                return Status == StatusStates.Generated;
            });
            return saveCommand;
        }

        private void InitEvaluator()
        {
            _graphEvaluator.ProgressChanged += (o, e) =>
            {
                ProgressPercent = e.ProgressPercentage;
                CurrentStep = e.CurrentStep;
                TotalSteps = e.TotalSteps;
            };
            _graphEvaluator.CliqueFound += (o, e) =>
            {
                _applicationState.MainThreadDispatcher.Invoke(() => 
                {
                    CliqueList.InsertSorted(new CliqueDataView(e.WordList));
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
            if (Status == StatusStates.NotGenerated)
            {
                _cancellationTokenSource?.Cancel();

                CliqueList.Clear();
                ProgressPercent = 0;
                CurrentStep = 0;
                TotalSteps = 0;

                _timer?.Stop();
                _loadingStopwatch?.Reset();
                UpdateElapsedTimeString();
            }

            SaveCommand.PostCanExecuteChanged();
        }

        private void CheckStartGenerating()
        {
            if (!IsActive)
                return;

            if (Status == StatusStates.Generating ||
                Status == StatusStates.Generated)
                return;

            if (!_applicationState.HasWordGraph ||
                _applicationState.WordGraph is null)
                return;

            Status = StatusStates.Generating;
            StartTimer();
            var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            _cancellationTokenSource = new CancellationTokenSource();
            _graphEvaluator.EvaluateGraphAsync(_applicationState.WordGraph, _cancellationTokenSource.Token)
                .ContinueWith(t =>
                {
                    StopTimer();
                    if (t.Exception == null &&
                        !t.IsCanceled)
                    {
                        Status = StatusStates.Generated;
                    }
                    else
                    {
                        Status = StatusStates.NotGenerated;
                    }
                }, uiScheduler);
        }

        #region Timer Methods
        private void StartTimer()
        {
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
        #endregion Timer Methods

        private void WriteCliques(string outputFile)
        {
            if (File.Exists(outputFile))
                File.Delete(outputFile);
            using (TextWriter writer = new StreamWriter(outputFile))
            {
                foreach (var wordList in CliqueList)
                {
                    writer.WriteLine(wordList.GetJoinedString("\t"));
                }
                writer.Close();
            }
        }
    }
}
