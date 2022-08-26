using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveWordFinder.WordProcessing.Model;

namespace FiveWordFinder.WordProcessing.Events
{
    public class WordProcessedEventArgs : EventArgs
    {
        public FiveCharWord Word { get; private set; }

        public WordProcessedEventArgs(FiveCharWord word)
        {
            Word = word;
        }
    }
}
