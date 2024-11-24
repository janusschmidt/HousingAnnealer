using Anneal;

namespace Tests;

public class HousingAnnealerTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestPenalty1()
    {
        int[][] board =
        [
            [1, 0, 1, 1, 0, 0, 0, 1],
            [1, 1, 1, 1, 0, 0, 0, 0],
            [1, 0, 0, 1, 1, 1, 0, 0],
            [1, 0, 1, 0, 0, 0, 1, 0],
            [1, 0, 0, 1, 1, 1, 0, 0],
            [0, 1, 1, 0, 0, 1, 1, 0],
            [1, 0, 1, 0, 0, 1, 1, 0],
            [0, 1, 0, 0, 1, 1, 0, 0]
        ];

        var penalty = HousingAnnealer.CalculatePenalty(board);
        Assert.That(penalty.Total, Is.EqualTo(11 * HousingAnnealer.TooManyNeighborsPenalty + 34 * HousingAnnealer.ExtraParkPenalty));
    }

    [Test]
    public void TestPenaltyTrivialBoard()
    {
        int[][] idealBoard =
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
        var penalty = HousingAnnealer.CalculatePenalty(idealBoard);
        Assert.That(penalty.Total, Is.EqualTo(32));
    }

    [Test]
    public void TestOptimalPenalty()
    {
        int[][] board =
        [
            [1, 0, 1, 1, 0, 1, 0, 1],
            [1, 0, 1, 0, 0, 1, 0, 1],
            [1, 0, 1, 1, 1, 1, 0, 1],
            [1, 0, 1, 0, 0, 1, 0, 1],
            [1, 0, 1, 1, 1, 1, 0, 1],
            [1, 0, 1, 0, 0, 1, 0, 1],
            [1, 0, 1, 1, 1, 1, 0, 1],
            [1, 0, 1, 0, 0, 1, 0, 1]
        ];

        var penalty = HousingAnnealer.CalculatePenalty(board);
        Assert.That(penalty.Total, Is.EqualTo(64 - 39));
    }
}