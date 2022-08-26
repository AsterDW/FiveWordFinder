using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveWordFinder.WordProcessing.Model;

namespace FiveWordFinder.WordProcessing.Events
{
    public class CliqueFoundEventArgs : EventArgs
    {
        public IEnumerable<string> WordList { get; private set; }

        public CliqueFoundEventArgs(IEnumerable<string> wordList)
        {
            WordList = wordList;
        }
    }
}
