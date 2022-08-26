using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveWordFinder.WordProcessing.Events;
using FiveWordFinder.WordProcessing.Model;

namespace FiveWordFinder.WordProcessing.Strategies
{
    public interface IGraphEvaluationStrategy : INotifyPropertyChanged
    {
        public double ProgressPercent { get; }
        public string ProgressMessage { get; }

        event EventHandler<ProgressChangedMessageEventArgs>? ProgressChanged;
        public IEnumerable<FiveCharWord[]> EvaluateGraph(WordGraph graph);
    }
}
