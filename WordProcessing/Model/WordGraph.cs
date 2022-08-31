using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveWordFinder.WordProcessing.Events;

namespace FiveWordFinder.WordProcessing.Model
{
    public enum GraphAddResults
    {
        AsNewWord,
        AsAnagram,
    }

    public class WordGraph
    {
        private HashSet<FiveCharWord> _wordSet;
        public IReadOnlySet<FiveCharWord> WordSet { get { return _wordSet; } }
        private Dictionary<int, FiveCharWord> _wordsByMask;
        public IReadOnlyDictionary<int, FiveCharWord> WordsByMask { get { return _wordsByMask; } }

        public int Count { get { return WordSet.Count; } }
        public int TotalNeighborsInGraph { get { return GetNeighborCount(); } }

        internal WordGraph()
        {
            _wordSet = new HashSet<FiveCharWord>();
            _wordsByMask = new Dictionary<int, FiveCharWord>();
        }

        internal WordGraph(int capacity)
        {
            _wordSet = new HashSet<FiveCharWord>(capacity);
            _wordsByMask = new Dictionary<int, FiveCharWord>(capacity);
        }

        private int GetNeighborCount()
        {
            return WordSet.Sum(v => v.Neighbors.Count);
        }

        public GraphAddResults AddWord(FiveCharWord word)
        {
            if (_wordsByMask.TryGetValue(word.CharBitMask, out var anagramParent))
            {
                anagramParent.AddAnagram(word);
                return GraphAddResults.AsAnagram;
            }
            
            _wordsByMask.Add(word.CharBitMask, word);
            _wordSet.Add(word);
            return GraphAddResults.AsNewWord;
        }
    }
}
