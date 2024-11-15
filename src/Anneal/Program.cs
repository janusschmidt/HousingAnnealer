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

      
      
      //PLINQ
      var best = Enumerable.Range(1, Annealer.MaxNumberOfExperiments)
        .AsParallel()
        .Select(_ => Annealer.GetRandomBoard())
        //.Select(_ => Annealer.GetIdealBoard())
        .Select(Annealer.Anneal)
        //.Where(x => x.Score.TooFewParksPenalty == 0)
        .OrderBy(x => x.Score.Total)
        .Take(5)
        .ToArray();

      //Parallel execution
      //var results = new ConcurrentBag<Annealer.Result>();
      // Parallel.For(1, Annealer.MaxNumberOfExperiments, (i, loopState) =>
      // {
      //     var b = Annealer.GetRandomBoard();
      //     var r = Annealer.Anneal(b);
      //     if (r.Score.TooFewParksPenalty == 0)
      //       results.Add(r);
      // });
      //var best = results.OrderBy(x => x.Score.Total).Take(10);

      Console.WriteLine($"Best scores:");

      foreach(var s in best) Annealer.PrintResult(s);

      Console.WriteLine($"Time taken: {sw.Elapsed}");
    }
  }
}


