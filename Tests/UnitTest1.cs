using Microsoft.VisualStudio.TestPlatform.TestHost;
using SimulatedAnnealing;

namespace Tests;

public class Tests
{
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {

        int[][] board = 
           [[1,0,1,1,0,0,0,1],
            [1,1,1,1,0,0,0,0],
            [1,0,0,1,1,1,0,0],
            [1,0,1,0,0,0,1,0],
            [1,0,0,1,1,1,0,0],
            [0,1,1,0,0,1,1,0],
            [1,0,1,0,0,1,1,0],
            [0,1,0,0,1,1,0,0]];
        
        var penalty = SimulatedAnnealing.Annealer.CalculatePenalty(board);
        Assert.That(penalty.TooFewParksPenalty, Is.EqualTo(0));
    }
    
    [Test]
    public void Test2()
    {
        var penalty = Annealer.CalculatePenalty(Annealer.GetIdealBoard());
        Assert.That(penalty.TooFewParksPenalty, Is.EqualTo(0));
    }
}