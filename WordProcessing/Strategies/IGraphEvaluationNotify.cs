using FiveWordFinder.WordProcessing.Events;
using FiveWordFinder.WordProcessing.Model;

namespace FiveWordFinder.WordProcessing.Strategies
{
    public interface IGraphEvaluationNotify : IGraphEvaluationStrategy
    {
        event EventHandler<CliqueFoundEventArgs>? CliqueFound;

        IEnumerable<FiveCharWord[]> EvaluateGraph(WordGraph graph, CancellationToken cancellationToken = default);
        Task<IEnumerable<FiveCharWord[]>> EvaluateGraphAsync(WordGraph graph, CancellationToken cancellationToken = default);
    }
}