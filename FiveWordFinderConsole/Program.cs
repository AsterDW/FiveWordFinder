using FiveWordFinderConsole.Services;
using FiveWordFinder.WordProcessing;
using FiveWordFinder.WordProcessing.Model;
using FiveWordFinder.WordProcessing.Strategies;

const string DefaultSourceFile = "Resources\\words_alpha.txt";
const string DefaultOutputFile = "Found_Cliques.txt";

InitConsole();
string filePath = ConsoleInputService.GetWordSourceFileName(DefaultSourceFile);
var evaluator = ConsoleInputService.GetEvaluationStrategy();
Console.WriteLine();
try
{
    var graph = GenerateWordGraph(filePath);
    var cliqueList = EvaluateGraph(graph, evaluator);
    WriteCliques(DefaultOutputFile, cliqueList);
}
catch(Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine("Press <enter> to close.");
Console.ReadLine();

void InitConsole()
{
    Console.Clear();
    Console.WriteLine("Find five english five letter words where every letter in the group is distinct.");
    Console.WriteLine("This problem was described by Matt Parker of Stand-up Maths in his YouTube video:");
    Console.WriteLine("\tCan you find: five five-letter words with twenty-five unique letters?");
    Console.WriteLine("\tPosted: Aug 3, 2022");
    Console.WriteLine("This was inspired when someone submitted a problem to his A Problem Squared podcast.");
    Console.WriteLine("\tHow many guesses can you have in Wordle before you have to repeat letters?");
    Console.WriteLine();
    Console.WriteLine("Let's Find Out");
    Console.WriteLine();
}

WordGraph GenerateWordGraph(string filePath)
{
    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    Console.WriteLine($"Generating Word Graph From File {filePath}");

    WordGraph graph;

    sw.Start();
    using (var progress = new ConsoleProgress())
    {
        GraphGenerator generator = new GraphGenerator();
        generator.ProgressChanged += (o, e) => { progress.Report((double)e.ProgressPercentage / 100, e.ProgressMessage); };
        graph = generator.GraphFromFile(filePath, 5);
        sw.Stop();
    }
    Console.WriteLine($"Complete: Loaded {graph.Count} words.");
    Console.WriteLine($"Found {graph.TotalNeighborsInGraph} Unique Neighbors.");
    Console.WriteLine($"Loading time: {sw.Elapsed}");
    Console.WriteLine();

    return graph;
}

IEnumerable<FiveCharWord[]> EvaluateGraph(WordGraph graph, IGraphEvaluationStrategy evaluator)
{
    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    Console.WriteLine("Finding Cliques:");
    sw.Start();
    IEnumerable<FiveCharWord[]> results;
    using (var progress = new ConsoleProgress())
    {
        evaluator.ProgressChanged += (o, e) => { progress.Report((double)e.ProgressPercentage / 100, e.ProgressMessage); };

        results = evaluator.EvaluateGraph(graph);

        sw.Stop();
    }

    Console.WriteLine($"Complete! Found {results.Count()} cliques.");
    Console.WriteLine($"Calculation time elapsed: {sw.Elapsed}");

    return results;
}

void WriteCliques(string outputFile, IEnumerable<FiveCharWord[]> cliqueList)
{
    if (File.Exists(outputFile))
        File.Delete(outputFile);
    using (TextWriter writer = new StreamWriter(outputFile))
    {
        foreach (var wordList in cliqueList.OrderBy(wl => wl.FirstOrDefault()))
        {
            string strings = string.Join<FiveCharWord>("\t", wordList);
            writer.WriteLine(strings);
            
            if (!Console.IsOutputRedirected)
                Console.WriteLine(strings);
        }
        writer.Close();
    }
    Console.WriteLine($"{cliqueList.Count()} cliques written to file: {outputFile}");
}