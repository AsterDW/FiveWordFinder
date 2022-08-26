using FiveWordFinder.WordProcessing.Events;
using FiveWordFinder.WordProcessing.Model;

namespace FiveWordFinder.WordProcessing
{
    public interface IGraphGenerator
    {
        event EventHandler<ProgressChangedMessageEventArgs>? ProgressChanged;
        event EventHandler<WordProcessedEventArgs>? WordAdded;

        WordGraph GraphFromEnumerable(IEnumerable<string> wordStrings, int wordLength);
        WordGraph GraphFromFile(string filePath, int wordLength);
        Task<WordGraph> GraphFromFileAsync(string filePath, int wordLength, CancellationToken cancellationToken = default);
    }
}