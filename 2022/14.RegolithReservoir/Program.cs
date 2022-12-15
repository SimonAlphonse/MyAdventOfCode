using System.Drawing;

namespace RegolithReservoir;

internal abstract class Program
{
    public static void Main()
    {
        var scanOne = File.ReadAllLines("inputs.txt")
            .Select(x => x.Split(" -> ").Select(y => y.Split(","))
                .Select(z => new Point(int.Parse(z.First()), int.Parse(z.Last()))).ToArray()).ToList();

        var source = new Point(0, 500);
        DrawRockSand(scanOne, source, out var drawing);
        DropSand(source, drawing, false);
        ExportToTextFile("PartOne.txt", drawing);
        Console.WriteLine($"Part One : {drawing.Cast<char>().Count(c => c == 'o')}");

        var scanTwo = ScanFloor(scanOne, drawing);
        DrawRockSand(scanTwo, source, out drawing);
        DropSand(new Point(-1, 500), drawing, true);
        ExportToTextFile("PartTwo.txt", drawing);
        Console.WriteLine($"Part Two : {drawing.Cast<char>().Count(c => c == 'o')}");

        Console.Read();
    }

    private static List<Point[]> ScanFloor(List<Point[]> scanOne, char[,] drawing)
    {
        var scanTwo = new List<Point[]>(scanOne);
        scanTwo.Add(new[]
        {
            new Point(0, drawing.GetLength(0) - 1 + 2),
            new Point(2 * drawing.GetLength(1) - 1, drawing.GetLength(0) - 1 + 2),
        });
        return scanTwo;
    }

    private static void DropSand(Point source, char[,] drawing, bool hasFloor)
    {
        var stop = false;
        var sand = new Point(source.X, source.Y);
        var path = new List<Point>();
        
        while (!stop)
        {
            path.Add(sand = sand with { X = sand.X + 1 });
            
            var left = drawing[sand.X + 1, sand.Y - 1];
            var middle = drawing[sand.X + 1, sand.Y];
            var right = drawing[sand.X + 1, sand.Y + 1];

            switch (left, middle, right)
            {
                case ('o' or '#', 'o' or '#', 'o' or '#'):
                    if (hasFloor && drawing[sand.X, sand.Y] == '+') 
                        stop = true;
                    drawing[sand.X, sand.Y] = 'o';
                    sand = new Point(source.X, source.Y);
                    path = new();
                    break;
                case ('.', 'o' or '#', _):
                    sand = sand with { X = sand.X - 1, Y = sand.Y - 1 };
                    break;
                case (_, 'o' or '#', '.'):
                    sand = sand with { X = sand.X - 1, Y = sand.Y + 1 };
                    break;
                case (_, '.', _): break;
                default: throw new ApplicationException("🤬");
            }

            if (sand.X + 1 + (hasFloor ? 0 : 1) == drawing.GetLength(0))
            {
                stop = true;
                path.Add(sand with { X = sand.X + 1 });
                foreach (var point in path)
                    drawing[point.X, point.Y] = '~';
            }
        }
    }

    private static void DrawRockSand(List<Point[]> scan, Point source, out char[,] drawing)
    {
        drawing = CreateChart(scan);
        drawing = DrawSand(drawing);
        drawing = DrawRock(scan, drawing);
        drawing[source.X, source.Y] = '+';
    }

    private static char[,] CreateChart(List<Point[]> scan)
    {
        var columns = scan.SelectMany(a => a.Select(b => b.X)).Max() + 1;
        var rows = scan.SelectMany(a => a.Select(b => b.Y)).Max() + 1;
        return new char[rows, columns];
    }

    private static char[,] DrawSand(char[,] drawing)
    {
        for (var i = 0; i < drawing.GetLength(0); i++)
        {
            for (var j = 0; j < drawing.GetLength(1); j++)
            {
                drawing[i, j] = '.';
            }
        }
        
        return drawing;
    }

    private static char[,] DrawRock(List<Point[]> scan, char[,] drawing)
    {
        foreach (var list in scan)
        {
            for (var j = 0; j < list.Length - 1; j++)
            {
                foreach (var point in list[j].GetPointsTill(list[j + 1]))
                {
                    drawing[point.Y, point.X] = '#';
                }
            }
        }

        return drawing;
    }

    private static void ExportToTextFile(string fileName, char[,] drawing)
    {
        File.Delete(fileName);

        using StreamWriter file = new(fileName, append: true);

        for (var i = 0; i < drawing.GetLength(0); i++)
        {
            for (var j = 0; j < drawing.GetLength(1); j++)
            {
                file.Write(drawing[i, j]);
            }

            file.Write(Environment.NewLine);
        }
    }
}

public static class PointExtensions
{
    public static Point[] GetPointsTill(this Point from, Point to)
    {
        if (from.Y == to.Y)
        {
            return Enumerable.Range(Math.Min(from.X, to.X),
                    Math.Max(from.X, to.X) - Math.Min(from.X, to.X) + 1)
                .Select(x => to with { X = x }).ToArray();
        }
        else if (from.X == to.X)
        {
            return Enumerable.Range(Math.Min(from.Y, to.Y),
                    Math.Max(from.Y, to.Y) - Math.Min(from.Y, to.Y) + 1)
                .Select(y => to with { Y = y }).ToArray();
        }

        return Array.Empty<Point>();
    }
}