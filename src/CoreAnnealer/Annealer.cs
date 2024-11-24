namespace CoreAnnealer;

public class Annealer<TSubject, TResult> where TResult : IComparable<TResult>
{
    const int maxIterations = 2_000;
    const double expFactor = 7;
    readonly Random rnd = new();

    public record struct Result(TSubject OptimalFoundSolution, TResult Score, int NumberOfChanges, int NumberOfIncreaseHeat);
    
    public Result Anneal(TSubject subject, Func<TSubject, TSubject> mutate, Func<TSubject, TResult> calculatePenalty)
    {
        var currentScore = calculatePenalty(subject);
        var numberOfChanges = 0;
        var numberOfIncreaseHeat = 0;
        for (var i = 0; i < maxIterations; i++)
        {
            var newSubject = mutate(subject);
            var newScore = calculatePenalty(newSubject);


            if (newScore.CompareTo(currentScore)>0)
            {
                if (AcceptJumpOutOfLocalMinimum(i))
                {
                    numberOfIncreaseHeat++;
                }
                else
                {
                    continue;
                }
            }

            currentScore = newScore;
            subject = newSubject;
            numberOfChanges++;
        }

        return new Result(subject, currentScore, numberOfChanges, numberOfIncreaseHeat);

        bool AcceptJumpOutOfLocalMinimum(int i)
        {
            var r = rnd.NextDouble();
            var comp = Math.Exp(-i * (expFactor / maxIterations));
            return r < comp;
        }
    }
}