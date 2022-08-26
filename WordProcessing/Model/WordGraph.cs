using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveWordFinder.WordProcessing.Events;

namespace FiveWordFinder.WordProcessing.Model
{
    public class WordGraph
    {
        public HashSet<FiveCharWord> WordSet { get; private set; }

        public int Count { get { return WordSet.Count; } }
        public int TotalNeighborsInGraph { get { return GetNeighborCount(); } }

        internal WordGraph()
        {
            WordSet = new HashSet<FiveCharWord>();
        }

        internal WordGraph(int capacity)
        {
            WordSet = new HashSet<FiveCharWord>(capacity);
        }

        private int GetNeighborCount()
        {
            return WordSet.Sum(v => v.Neighbors.Count);
        }
    }
}
