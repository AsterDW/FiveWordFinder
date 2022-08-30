using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FiveWordFinder.WordProcessing.Model;
using FiveWordFinder.Extensions;

namespace FiveWordFinder.WordProcessing.Strategies
{
    public class EvaluateGraphParallel : baseGraphEvaluatorStrategy
    {
        private int i = 0;
        private int TotalCount = 0;

        private object resultsLock = new object();

        /// <summary>
        /// Initiates the graph evaluation with the first loop over the words of a given graph set.
        /// This recursively scans each word and their neighbors culling the possible candidate list
        /// with each level. Only words which are contained in all 5 word's neighbors sets are valid
        /// combinations of words with unique letters across the group.
        /// Runs the foreach loop as a parallel operation.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public override IEnumerable<FiveCharWord[]> EvaluateGraph(WordGraph graph)
        {
            Reset(graph.WordSet.Count);

            Parallel.ForEach(graph.WordSet, FiveCharWordModel =>
            {
                Interlocked.Increment(ref i);

                OnProgressChanged(GetPercent(), i, TotalCount, GetMessage());

                Stack<FiveCharWord> wordStack = new Stack<FiveCharWord>();
                wordStack.Push(FiveCharWordModel);
                recursiveFindCliques(5, wordStack, FiveCharWordModel.Neighbors);
                wordStack.Pop();
            });


            return CliqueList;
        }

        /// <summary>
        /// Recursive function to scan each word in a given graph for common neighbors by calling the function again with
        /// an intersection of the given word's neighbors and the next word's neighbors up to the number of itterations 
        /// specified by the depth parameter.
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="wordStack"></param>
        /// <param name="clique"></param>
        private void recursiveFindCliques(int depth, Stack<FiveCharWord> wordStack, IEnumerable<FiveCharWord> clique)
        {
            foreach (var word in clique)
            {
                wordStack.Push(word);
                if (wordStack.Count == depth)
                {
                    var wordArrays = GetStackCombinations(wordStack);
                    lock (resultsLock)
                    {
                        foreach (var a in wordArrays)
                        {
                            CliqueList.Add(a.ToArray());
                        }
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

            Reset();
        }

        private int GetPercent()
        {
            return CalcPercent(i, TotalCount);
        }

        private string GetMessage()
        {
            return $"Cliques Found: {CliqueList.Count}";
        }
    }
}
