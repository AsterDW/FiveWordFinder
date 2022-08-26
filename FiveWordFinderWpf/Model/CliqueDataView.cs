using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FiveWordFinderWpf.Model
{
    public class CliqueDataView : IComparable<CliqueDataView>
    {
        private List<string> _wordsList;
		public IEnumerable<string> Words => _wordsList.AsEnumerable();

        public int WordCount => _wordsList.Count;

        public string Word1
        {
            get { return _wordsList.ElementAtOrDefault(0) ?? string.Empty; }
        }
        public string Word2
        {
            get { return _wordsList.ElementAtOrDefault(1) ?? string.Empty; }
        }
        public string Word3
        {
            get { return _wordsList.ElementAtOrDefault(2) ?? string.Empty; }
        }
        public string Word4
        {
            get { return _wordsList.ElementAtOrDefault(3) ?? string.Empty; }
        }
        public string Word5
        {
            get { return _wordsList.ElementAtOrDefault(4) ?? string.Empty; }
        }

        public CliqueDataView(IEnumerable<string> words)
        {
            _wordsList = new List<string>(words);
        }

        public string GetJoinedString(string joinSeparator)
        {
            return string.Join(joinSeparator, _wordsList);
        }

        public int CompareTo(CliqueDataView? other)
        {
            // If other is not a valid object reference, this instance is greater.
            if (other == null) return 1;
            int comp = 0;
            
            for(int i = 0; comp == 0 && i < _wordsList.Count; i++)
            {
                comp = _wordsList[i].CompareTo(other._wordsList[i]);
            }

            return comp;
        }
    }
}
