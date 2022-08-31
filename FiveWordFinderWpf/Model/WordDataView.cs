using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FiveWordFinder.WordProcessing.Model;

namespace FiveWordFinderWpf.Model
{
    public class WordDataView : INotifyPropertyChanged
    {
        private FiveCharWord _word;
        public string Word { get { return _word.Word; } }

        public int LetterCount { get; private set; }

        public IReadOnlyList<string> Anagrams { get { return _word.Anagrams.Select(a => a.Word).ToList(); } }

        public int NeighborsCount { get { return _word.Neighbors.Count; } }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged

        public WordDataView(FiveCharWord word)
        {
            _word = word;
            LetterCount = _word.CountUniqueLetters;

            _word.PropertyChanged += word_PropertyChanged;
        }

        private void word_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_word.Anagrams))
            {
                OnPropertyChanged(nameof(Anagrams));
            }
            else if (e.PropertyName == nameof(_word.Neighbors))
            {
                OnPropertyChanged(nameof(NeighborsCount));
            }
        }
    }
}
