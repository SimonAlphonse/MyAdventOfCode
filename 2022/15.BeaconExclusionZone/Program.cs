using System.Drawing;

namespace BeaconExclusionZone;

internal abstract class Program
{
    record Spot(Point From, Point To);

    public static void Main()
    {
        var inputs = File.ReadAllLines($"inputs-demo.txt")
            .Select(a => a.Split(' '))
            .Select(b => new Spot(
                new(int.Parse(b[2][2..^1]), int.Parse(b[3][2..^1])),
                new(int.Parse(b[8][2..^1]), int.Parse(b[9][2..]))));

        
    }
}