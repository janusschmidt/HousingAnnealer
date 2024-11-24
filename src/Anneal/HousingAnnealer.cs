namespace Anneal;

public class HousingAnnealer
{
    class Annealer : CoreAnnealer.Annealer<int[][], Score>;

    public readonly record struct Score(int TooManyParksPenalty, int TooFewParksPenalty, int Total) : IComparable<Score>
    {
        public int CompareTo(Score other) => Total.CompareTo(other.Total);
    }

    public const int MaxNumberOfExperiments = 2_000;
    public const int TooManyNeighborsPenalty = 1000;
    public const int ExtraParkPenalty = 1;
    static readonly Random Rnd = new();
    
    readonly Annealer annealer = new();
    
    public Annealer.Result Anneal() => annealer.Anneal(RandomBoard, Mutate, CalculatePenalty);

    static int[][] Mutate(int[][] board)
    {
        return Rnd.Next(1, 3) == 1 ? MutateSwap(board) : MutatePoint(board);
    }

    static int[][] MutatePoint(int[][] board)
    {
        var col = Rnd.Next(8);
        var row = Rnd.Next(8);

        var newBoard = DeepClone(board);
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

            var newBoard = DeepClone(board);
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
        for (var i = startRow; i <= endRow; i++)
        for (var j = startCol; j <= endCol; j++)
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

    public static void PrintResult(params Annealer.Result?[] results)
    {
        foreach (var result in results)
        {
            if (!result.HasValue) 
                continue;
            
            var (bestBoard, bestScore, numberOfChanges, numberOfIncreaseHeat) = result.Value;
            Console.WriteLine($"Solution with score: {bestScore.Total}");
            Console.WriteLine($"Solution with {nameof(bestScore.TooFewParksPenalty)} penalty: {bestScore.TooFewParksPenalty}");
            Console.WriteLine($"Solution with {nameof(bestScore.TooManyParksPenalty)} penalty: {bestScore.TooManyParksPenalty}. Ie. number of houses = {8 * 8 - bestScore.TooManyParksPenalty}");
            Console.WriteLine($"Number of accepted changes: {numberOfChanges}");
            Console.WriteLine($"Number of accepted increase temperature: {numberOfIncreaseHeat}");

            foreach (var ser in bestBoard)
            {
                Console.WriteLine(string.Join(",", ser));
            }

            Console.WriteLine("********************************************");
        }
    }
    
    static int[][] RandomBoard =>
        Enumerable.Range(1, 8).Select(_ => Enumerable.Range(1, 8).Select(_ => Rnd.Next(2)).ToArray()).ToArray();
    
    static int[][] DeepClone(int[][] board) => board.Select(x => x.ToArray()).ToArray();
}