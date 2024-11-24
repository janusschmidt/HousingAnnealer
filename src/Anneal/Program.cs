using System.Diagnostics;

namespace Anneal;

static class Program
{
    static void Main()
    {
        Console.WriteLine("Started...");
        var annealer = new HousingAnnealer();

        var sw = new Stopwatch();
        sw.Start();

        var best = Enumerable.Range(1, HousingAnnealer.MaxNumberOfExperiments)
            .AsParallel()
            .Select(_ => annealer.Anneal())
            .OrderBy(x => x.Score.Total)
            .Take(3)
            .ToArray();

        Console.WriteLine($"Time taken: {sw.Elapsed}");
        Console.WriteLine($"Best scores:");

        foreach (var s in best) HousingAnnealer.PrintResult(s);

        Console.WriteLine($"Time taken: {sw.Elapsed}");
    }
}