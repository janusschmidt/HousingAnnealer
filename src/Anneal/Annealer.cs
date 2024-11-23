namespace SimulatedAnnealing;

public static class Annealer
{
    static readonly Random Rnd = new();
    const int maxIterations = 2_000;
    const double expFactor = 7;
    public const int MaxNumberOfExperiments = 2_000;
    public const int TooManyNeighborsPenalty = 1000;
    public const int ExtraParkPenalty = 1;

    public record struct Score(int TooManyParksPenalty, int TooFewParksPenalty, int Total);

    public record struct Result(int[][] Board, Score Score, int NumberOfChanges, int NumberOfIncreaseHeat);

    public static Result Anneal() => Anneal(RandomBoard);

    static Result Anneal(int[][] board)
    {
        var currentScore = CalculatePenalty(board);
        var numberOfChanges = 0;
        var numberOfIncreaseHeat = 0;
        for (var i = 0; i < maxIterations; i++)
        {
            var newBoard = Mutate(board);
            var newScore = CalculatePenalty(newBoard);


            if (newScore.Total > currentScore.Total)
            {
                var acceptJumpOutOfLocalMinimum = AcceptJumpOutOfLocalMinimum(i);
                if (acceptJumpOutOfLocalMinimum)
                {
                    numberOfIncreaseHeat++;
                }
                else
                {
                    continue;
                }
            }

            currentScore = newScore;
            board = newBoard;
            numberOfChanges++;
        }

        return new Result(board, currentScore, numberOfChanges, numberOfIncreaseHeat);

        bool AcceptJumpOutOfLocalMinimum(int i)
        {
            var r = Rnd.NextDouble();
            var comp = Math.Exp(-i * (expFactor / maxIterations));
            return r < comp;
        }
    }

    static int[][] Mutate(int[][] board)
    {
        return Rnd.Next(1, 3) == 1 ? MutateSwap(board) : MutatePoint(board);
    }

    static int[][] MutatePoint(int[][] board)
    {
        var col = Rnd.Next(8);
        var row = Rnd.Next(8);

        var newBoard = board.CloneX();
        newBoard[row][col] = newBoard[row][col] == 1 ? 0 : 1;
        return newBoard;
    }

    static int[][] MutateSwap(int[][] board)
    {
        for (var i = 0; i < 10; i++)
        {
            var col = Rnd.Next(1, 7);
            var row = Rnd.Next(1, 7);

            var upDown = Rnd.Next(1, 4) switch
            {
                1 => -1,
                2 => 0,
                3 => 1,
                _ => throw new ArgumentOutOfRangeException()
            };

            var leftRight = Rnd.Next(1, 4) switch
            {
                1 => -1,
                2 => 0,
                3 => 1,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (upDown == 0 && leftRight == 0)
                continue;

            var newBoard = board.CloneX();
            var p1 = newBoard[row][col];
            var p2 = newBoard[row + upDown][col + leftRight];
            if (p1 == p2)
                continue;

            newBoard[row][col] = p2;
            newBoard[row + upDown][col + leftRight] = p1;
            return newBoard;
        }

        return board;
    }

    public static Score CalculatePenalty(int[][] board)
    {
        var tooManyParksPenalty = board.SelectMany(x => x.Select(y => y)).Count(x => x == 0) * ExtraParkPenalty;
        var tooManyNeighbors =
            board.Select((row, rowindex) =>
                    row.Where((col, colindex) => col == 1 && HasTooManyNeighbors(rowindex, colindex, board)).Count())
                .Sum() * TooManyNeighborsPenalty;
        return new Score(tooManyParksPenalty, tooManyNeighbors, tooManyParksPenalty + tooManyNeighbors);
    }

    static bool HasTooManyNeighbors(int rowindex, int colindex, int[][] board)
    {
        var startRow = rowindex == 0 ? 0 : rowindex - 1;
        var endRow = rowindex == 7 ? 7 : rowindex + 1;

        var startCol = colindex == 0 ? 0 : colindex - 1;
        var endCol = colindex == 7 ? 7 : colindex + 1;

        var neighbors = 0;
        var parks = 0;
        for (int i = startRow; i <= endRow; i++)
        for (int j = startCol; j <= endCol; j++)
        {
            if (i == rowindex && j == colindex)
                continue;

            if (board[i][j] == 1)
                neighbors++;
            else
                parks++;
        }

        return neighbors > parks;
    }

    public static void PrintResult(params Result?[] results)
    {
        foreach (var result in results)
        {
            if (result.HasValue)
            {
                var (bestBoard, bestScore, numberOfChanges, numberOfIncreaseHeat) = result.Value;
                Console.WriteLine($"Solution with score: {bestScore.Total}");
                Console.WriteLine(
                    $"Solution with {nameof(bestScore.TooFewParksPenalty)} penalty: {bestScore.TooFewParksPenalty}");
                Console.WriteLine(
                    $"Solution with {nameof(bestScore.TooManyParksPenalty)} penalty: {bestScore.TooManyParksPenalty}. Ie. number of houses = {8 * 8 - bestScore.TooManyParksPenalty}");
                Console.WriteLine($"Number of accepted changes: {numberOfChanges}");
                Console.WriteLine($"Number of accepted increase temperature: {numberOfIncreaseHeat}");

                foreach (var ser in bestBoard)
                {
                    Console.WriteLine(string.Join(",", ser));
                }

                Console.WriteLine("********************************************");
            }
        }
    }

    static int[][] CloneX(this int[][] board) => board.Select(x => x.ToArray()).ToArray();
    
    static int[][] RandomBoard =>
        Enumerable.Range(1, 8).Select(_ => Enumerable.Range(1, 8).Select(_ => Rnd.Next(2)).ToArray()).ToArray();
}