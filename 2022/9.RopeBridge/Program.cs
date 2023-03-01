using System.Drawing;

namespace RopeBridge;

internal abstract class Program
{
    record Instruction(string Direction, int Distance);

    public static void Main()
    {
        Console.WriteLine($"Part One Demo : {GetUniqueTrailCount(GetInstructions("demo-inputs-1.txt"), 2)}");
        Console.WriteLine($"Part One : {GetUniqueTrailCount(GetInstructions("inputs.txt"), 2)}");
        Console.WriteLine($"Part Two Demo : {GetUniqueTrailCount(GetInstructions("demo-inputs-2.txt"), 10)}");
        Console.WriteLine($"Part Two : {GetUniqueTrailCount(GetInstructions("inputs.txt"), 10)}");

        Console.ReadKey();
    }

    private static List<Instruction> GetInstructions(string filename)
    {
        return File.ReadAllLines(filename)
            .Select(s => s.Split(" ").ToArray())
            .Select(s => new Instruction(s.First(), int.Parse(s.Last()))).ToList();
    }

    private static int GetUniqueTrailCount(List<Instruction> inputs, int length)
    {

        List<Point>[] trails = Enumerable.Range(0, length)
            .Select(_ => new List<Point>() { new(0, 0) }).ToArray();

        foreach (var instruction in inputs)
        {
            for (var i = 1; i <= instruction.Distance; i++)
            {
                trails[0].Add(MoveHead(trails[0].Last(), instruction));

                for (var index = 1; index < length; index++)
                {
                    var head = trails[index - 1];
                    var tail = trails[index];

                    tail.Add(MoveTail(head.Last(), tail.Last()));
                }
            }
        }

        return trails.Last().Distinct().Count();
    }

    private static Point MoveHead(Point head, Instruction instruction)
    {
        return instruction.Direction switch
        {
            "R" => head with { X = head.X + 1 },
            "U" => head with { Y = head.Y + 1 },
            "L" => head with { X = head.X - 1 },
            _ => head with { Y = head.Y - 1 }
        };
    }

    private static Point MoveTail(Point head, Point tail)
    {
        int xDiff = head.X - tail.X, yDiff = head.Y - tail.Y;

        return Math.Abs(xDiff) > 1 || Math.Abs(yDiff) > 1
            ? new Point(tail.X + Math.Sign(xDiff), tail.Y + Math.Sign(yDiff))
            : tail;
    }
}