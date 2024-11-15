namespace SimulatedAnnealing;


public static class Annealer
{
    static readonly Random Rnd = new();
    const int maxIterations = 4_000;
    const double expFactor = 7;
    public const int MaxNumberOfExperiments = 160_000;
    const int tooManyNeighborsPenalty = 1000;

    public record struct Score(int TooManyParksPenalty, int TooFewParksPenalty, int Total);
    public record struct Result(int[][] Board, Score Score, int NumberOfChanges);

    public static Result Anneal(int[][] board)
    {
        var bestBoard = board;
        var bestScore = CalculatePenalty(board);
        var numberOfChanges = 0;
        for (var i = 0; i < maxIterations; i++)
        {
            var newBoard = Mutate(board);
            var newScore = CalculatePenalty(newBoard);
            
            if (newScore.Total >= bestScore.Total && !AcceptJumpOutOfLocalMinimum(i)) 
                continue;
            
            bestScore = newScore;
            board = newBoard;
            numberOfChanges++;
        }
        return new Result(bestBoard, bestScore, numberOfChanges);

        bool AcceptJumpOutOfLocalMinimum(int i)
        {
            var r = Rnd.NextDouble();
            var comp = Math.Exp(-i * (expFactor / maxIterations));
            return r < comp;
        }
    }

    static int[][] Mutate(int[][] board)
    {
        var col = Rnd.Next(8);
        var row = Rnd.Next(8);
        var newBoard = board.Select(r => r.ToArray()).ToArray();
        newBoard[row][col] = newBoard[row][col] == 1 ? 0 : 1;
        return newBoard;
    }

    public static Score CalculatePenalty(int[][] board)
    {
        var tooManyParksPenalty = board.SelectMany(x => x.Select(y=>y)).Count(x => x == 0);
        var tooManyNeighbors = board.Select((row,rowindex)=> row.Select((col,colindex)=> col == 1 && HasTooManyNeighbors(rowindex, colindex, board)).Count(b=>b)).Sum() * tooManyNeighborsPenalty;
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
        for (int i = startRow; i<=endRow; i++)
        for (int j = startCol; j<=endCol; j++)
        {
            if (i==rowindex && j==colindex) 
                continue;
 
            if (board[i][j] == 1)
                neighbors++;
            else
                parks++;
        }

        return neighbors>parks;
    }

    public static void PrintResult(params Result?[] results)
    {
        foreach (var result in results)
        {
            if (result.HasValue)
            {
                var (bestBoard, bestScore, numberOfChanges) = result.Value;
                Console.WriteLine($"Solution with score: {bestScore.Total}");
                Console.WriteLine($"Solution with {nameof(bestScore.TooFewParksPenalty)} penalty: {bestScore.TooFewParksPenalty}");
                Console.WriteLine($"Solution with {nameof(bestScore.TooManyParksPenalty)} penalty: {bestScore.TooManyParksPenalty}. Ie. number of houses = {8*8-bestScore.TooManyParksPenalty}");
                Console.WriteLine($"Number of accepted changes: {numberOfChanges}");

                foreach (var ser in bestBoard)
                {
                    Console.WriteLine(string.Join(",", ser));
                }

                Console.WriteLine("********************************************");
            }
        }
    }

    public static int[][] GetRandomBoard()
    {
        return Enumerable.Range(1,8).Select(_=> Enumerable.Range(1,8).Select(_ => Rnd.Next(2)).ToArray()).ToArray();
    }

    public static int[][] GetIdealBoard()
    {
        return
        [
            [1, 0, 1, 0, 1, 0, 1, 0],
            [0, 1, 0, 1, 0, 1, 0, 1],
            [1, 0, 1, 0, 1, 0, 1, 0],
            [0, 1, 0, 1, 0, 1, 0, 1],
            [1, 0, 1, 0, 1, 0, 1, 0],
            [0, 1, 0, 1, 0, 1, 0, 1], 
            [1, 0, 1, 0, 1, 0, 1, 0],
            [0, 1, 0, 1, 0, 1, 0, 1],
        ];
    }
}