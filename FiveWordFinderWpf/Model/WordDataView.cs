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
    public class WordDataView
    {
        public string Word { get; private set; }

        public int LetterCount { get; private set; }

        public int NeighborsCount { get; private set; }

        public WordDataView(string word, int letterCount, int neighborsCount)
        {
            Word = word;
            LetterCount = letterCount;
            NeighborsCount = neighborsCount;
        }
    }
}
