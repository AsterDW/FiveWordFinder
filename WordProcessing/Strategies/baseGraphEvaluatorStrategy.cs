using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FiveWordFinder.WordProcessing.Events;
using FiveWordFinder.WordProcessing.Model;

namespace FiveWordFinder.WordProcessing.Strategies
{
    public abstract class baseGraphEvaluatorStrategy : IGraphEvaluationStrategy
    {
        private double _progressPercent = 0;
        private string _progressMessage = string.Empty;

        public double ProgressPercent 
        { 
            get { return _progressPercent; } 
            private set 
            { 
                _progressPercent = value;
                OnPropertyChanged();
            } 
        }

        public string ProgressMessage
        {
            get { return _progressMessage; }
            private set
            {
                _progressMessage = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler<ProgressChangedMessageEventArgs>? ProgressChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        protected List<FiveCharWord[]> CliqueList = new List<FiveCharWord[]>();

        public abstract IEnumerable<FiveCharWord[]> EvaluateGraph(WordGraph graph);

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "" )
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs( propertyName ) );
        }

        protected void OnProgressChanged(int percent, int? currentStep, int? totalSteps, string message)
        {
            ProgressPercent = percent;
            ProgressMessage = message;

            ProgressChanged?.Invoke(this, new ProgressChangedMessageEventArgs( percent, currentStep, totalSteps, message, null ) );
        }

        protected int CalcPercent(int current, int total)
        {
            return (int)((double)current / total * 100);
        }

        protected virtual void Reset()
        {
            CliqueList.Clear();
            OnProgressChanged(0, 0, 0, string.Empty);
        }
    }
}
