using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiveWordFinder.WordProcessing.Events;
using FiveWordFinder.WordProcessing.Model;

namespace FiveWordFinder.WordProcessing
{
    public class GraphGenerator : IGraphGenerator
    {

        public event EventHandler<ProgressChangedMessageEventArgs>? ProgressChanged;

        public event EventHandler<WordProcessedEventArgs>? WordAdded;

        private void OnProgressChanged(int progressPercent, int? curStep, int? totSteps, string message)
        {
            ProgressChanged?.Invoke(this, new ProgressChangedMessageEventArgs(progressPercent, curStep, totSteps, message, null));
        }

        private void OnWordAdded(FiveCharWord word)
        {
            WordAdded?.Invoke(this, new WordProcessedEventArgs(word));
        }

        public async Task<WordGraph> GraphFromFileAsync(string filePath,
                                                                int wordLength,
                                                                CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Task.Run(() => GenerateFromFile(filePath, wordLength, cancellationToken));
        }

        public WordGraph GraphFromFile(string filePath, int wordLength)
        {
            return GenerateFromFile(filePath, wordLength);
        }

        private WordGraph GenerateFromFile(string filePath, int wordLength, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("The specified file does not exist.", filePath);

            SortedSet<FiveCharWord> wordSet = new SortedSet<FiveCharWord>();

            FileInfo fileInfo = new FileInfo(filePath);
            using (var stream = fileInfo.OpenRead())
            {
                using (var streamReader = new StreamReader(stream))
                {
                    string? line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            cancellationToken.ThrowIfCancellationRequested();

                        OnProgressChanged((int)(stream.Position / stream.Length * 100), null, null,
                                          $"Reading words from file: {line}");

                        if (line.Length != wordLength)
                            continue;

                        FiveCharWord wm = new FiveCharWord(line);
                        if (wm.CountUniqueLetters == wordLength)
                            wordSet.Add(wm);
                    }
                }
                OnProgressChanged(100, null, null,
                                  $"Reading words from file:");
            }

            WordGraph graph = new WordGraph(wordSet.Count);
            FillGraph(graph, wordSet, cancellationToken);

            return graph;
        }

        private void FillGraph(WordGraph graph, SortedSet<FiveCharWord> wordSet, CancellationToken cancellationToken = default(CancellationToken))
        {
            int i = 0;
            //Add the words to the graph set and find words that do not share letters in common.
            //These are stored as Neighbors based on the bit flag evaluation for the characters in a given word.
            foreach (var word in wordSet)
            {
                i++;
                OnProgressChanged((int)((double)i / wordSet.Count * 100), i, wordSet.Count,
                                  $"Finding Neighbors");

                //Deduplication: As later we only consider words in order where i < j < k < l < m
                //to prevent repeated combinations of words in a different order. So do not add
                //neighbors who are alphabetically lower than the current word.
                foreach (var word2 in wordSet.Where(w => w > word))
                {
                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    //Using the AddNeighbor method of the WordModel which checks the bitmasks of the two
                    //words before adding as a neighbor.
                    word.AddNeighbor(word2);
                }

                graph.WordSet.Add(word);
                OnWordAdded(word);
            }
        }

        public WordGraph GraphFromEnumerable(IEnumerable<string> wordStrings, int wordLength)
        {
            //Sort the incoming word strings from the input list in case they are unordered.
            SortedSet<FiveCharWord> wordSet = new SortedSet<FiveCharWord>();
            int i = 0;
            int count = wordStrings.Count();

            foreach (var word in wordStrings)
            {
                i++;
                OnProgressChanged((int)((double)i / count * 100), i, count,
                                  $"Reading words: {word}");

                if (word.Length != wordLength)
                    continue;

                FiveCharWord w = new FiveCharWord(word);
                if (w.CountUniqueLetters == wordLength)
                    wordSet.Add(w);
            }

            WordGraph graph = new WordGraph(wordSet.Count);
            FillGraph(graph, wordSet);

            return graph;
        }
    }
}
