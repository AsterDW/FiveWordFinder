using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FiveWordFinder.Extensions;
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

        protected IEnumerable<IEnumerable<FiveCharWord>> GetStackCombinations(Stack<FiveCharWord> wordStack)
        {
            var words = wordStack.ToArrayFiFo();
            var groups = new List<IEnumerable<FiveCharWord>>();

            foreach (var word in words)
            {
                List<FiveCharWord> group = new List<FiveCharWord>();
                group.Add(word);
                group.AddRange(word.Anagrams);
                groups.Add(group);
            }

            return CartesianProducts(groups, 0);
        }

        protected IEnumerable<IEnumerable<FiveCharWord>> CartesianProducts(IList<IEnumerable<FiveCharWord>> lists, int depth, Stack<FiveCharWord>? product = null)
        {
            product = product ?? new Stack<FiveCharWord>();

            foreach (var word in lists[depth])
            {
                product.Push(word);

                if (product.Count == lists.Count)
                {
                    yield return product.ToArray().OrderBy(w => w);
                }
                else
                {
                    foreach (var pItem in CartesianProducts(lists, depth + 1, product))
                    {
                        yield return pItem;
                    }
                }

                product.Pop();
            }
        }
    }
}
