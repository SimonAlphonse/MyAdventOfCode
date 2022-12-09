
using System.Drawing;

internal class Program
{
    record Instruction(string Direction, int Distance);

    public static void Main()
    {
        Console.WriteLine($"Part One Demo: {GetUniqueTrailCount(GetInstructions("demo-inputs-1.txt"), 2)}");
        Console.WriteLine($"Part One : {GetUniqueTrailCount(GetInstructions("inputs.txt"), 2)}");
        Console.WriteLine($"Part Two Demo : {GetUniqueTrailCount(GetInstructions("demo-inputs-2.txt"), 10)}");
        Console.WriteLine($"Part Two : {GetUniqueTrailCount(GetInstructions("inputs.txt"), 10)}");

        Console.Read();
    }

    private static List<Instruction> GetInstructions(string filename)
    {
        return File.ReadAllLines(filename)
            .Select(s => s.Split(" ").ToArray())
            .Select(s => new Instruction(s.First(), int.Parse(s.Last()))).ToList();
    }

    private static int GetUniqueTrailCount(List<Instruction> inputs, int length)
    {
        List<List<Point>> trails = Enumerable.Range(0, length)
            .Select(key => new List<Point>() { new(0, 0) }).ToList();

        foreach (var instruction in inputs)
        {
            for (var i = 1; i <= instruction.Distance; i++)
            {
                for (var index = 0; index < length - 1; index++)
                {
                    var head = trails[index];
                    var tail = trails[index + 1];
                    var headTrail = head.Last();

                    if (index == 0)
                    {
                        headTrail = MoveHead(head.Last(), instruction);
                        head.Add(headTrail);
                    }

                    tail.Add(MoveTail(headTrail, tail.Last()));
                }
            }
        }

        return trails.Last().Distinct().Count();
    }

    private static Point MoveHead(Point head, Instruction instruction)
    {
        return instruction.Direction switch
        {
            "L" => head with { X = head.X - 1 },
            "R" => head with { X = head.X + 1 },
            "U" => head with { Y = head.Y + 1 },
            _ => head with { Y = head.Y - 1 }
        };
    }

    private static Point MoveTail(Point head, Point tail)
    {
        int tailX = tail.X, tailY = tail.Y;

        if (head.X - tailX > 1)
        {
            tailX++;
            tailY = head.Y;
        }
        else if (tailX - head.X > 1)
        {
            tailX--;
            tailY = head.Y;
        }

        if (head.Y - tailY > 1)
        {
            tailY++;
            tailX = head.X;
        }
        else if (tailY - head.Y > 1)
        {
            tailY--;
            tailX = head.X;
        }

        return new(tailX, tailY);
    }
}