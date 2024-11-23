using System.Diagnostics;

namespace SimulatedAnnealing
{
    static class Program
    {
        static void Main()
        {
            Console.WriteLine("Started...");

            var sw = new Stopwatch();
            sw.Start();

            var best = Enumerable.Range(1, Annealer.MaxNumberOfExperiments)
                .AsParallel()
                .Select(_ => Annealer.Anneal())
                .OrderBy(x => x.Score.Total)
                .Take(3)
                .ToArray();

            Console.WriteLine($"Best scores:");

            foreach (var s in best) Annealer.PrintResult(s);

            Console.WriteLine($"Time taken: {sw.Elapsed}");
        }
    }
}