using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveWordFinder.WordProcessing.Model;
using FiveWordFinder.Extensions;

namespace FiveWordFinder.WordProcessing.Strategies
{
    public class EvaluateGraphSeries : baseGraphEvaluatorStrategy
    {
        private int i = 0;
        private int TotalCount = 0;
        private string CurrentWord = string.Empty;

        public override IEnumerable<FiveCharWord[]> EvaluateGraph(WordGraph graph)
        {
            Reset(graph.WordSet.Count);

            foreach (var iWord in graph.WordSet)
            {
                i++;
                CurrentWord = iWord.Word;
                OnProgressChanged(GetPercent(), i, TotalCount, GetMessage());

                Stack<FiveCharWord> wordStack = new Stack<FiveCharWord>();
                wordStack.Push(iWord);

                recursiveFindCliques(5, wordStack, iWord.Neighbors);

                wordStack.Pop();
            }

            return CliqueList;
        }

        private void recursiveFindCliques(int depth, Stack<FiveCharWord> wordStack, IEnumerable<FiveCharWord> clique)
        {
            foreach (var word in clique)
            {
                wordStack.Push(word);
                if (wordStack.Count == depth)
                {
                    var wordArrays = GetStackCombinations(wordStack);
                    foreach (var a in wordArrays)
                    {
                        CliqueList.Add(a.ToArray());
                    }
                }
                else
                {
                    recursiveFindCliques(depth, wordStack, clique.IntersectWith(word.Neighbors));
                }

                wordStack.Pop();
            }
        }

        private void Reset(int totalCount)
        {
            i = 0;
            TotalCount = totalCount;
            CurrentWord = string.Empty;

            Reset();
        }

        private int GetPercent()
        {
            return CalcPercent(i, TotalCount);
        }

        private string GetMessage()
        {
            return $"Cliques Found: {CliqueList.Count} - [{CurrentWord}]";
        }
    }
}
