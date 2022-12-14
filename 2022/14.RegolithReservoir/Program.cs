using System.Drawing;

namespace RegolithReservoir;

internal abstract class Program
{
    public static void Main()
    {
        var inputs = File.ReadAllLines("inputs.txt")
            .Select(x => x.Split(" -> ")
                .Select(y => y.Split(","))
                .Select(z => new Point(int.Parse(z.First()), int.Parse(z.Last())))
                .ToArray()).ToArray();

        var source = CreateMap(inputs, out var map);

    }

    private static Point CreateMap(Point[][] inputs, out char[,] map)
    {
        var columns = inputs.SelectMany(a => a.Select(b => b.X)).ToArray().Max() + 1 + 10;
        var rows = inputs.SelectMany(a => a.Select(b => b.Y)).ToArray().Max() + 1 + 10;
        map = new char[rows, columns];

        for (var i = 0; i < map.GetLength(0); i++)
        {
            for (var j = 0; j < map.GetLength(1); j++)
            {
                map[i, j] = '.';
            }
        }
        
        var source = new Point(0, 500);
        map[source.X, source.Y] = '+';

        foreach (var list in inputs)
        {
            for (var j = 0; j < list.Length - 1; j++)
            {
                foreach (var point in list[j].Till(list[j + 1]))
                {
                    map[point.Y, point.X] = '#';
                }
            }
        }
        
        File.Delete("Print.txt");
        
        using StreamWriter file = new("Print.txt", append: true);

        for (var i = 0; i < map.GetLength(0); i++)
        {
            for (var j = 0; j < map.GetLength(1); j++)
            {
                file.Write(map[i, j]);
            }

            file.Write(Environment.NewLine);
        }
        
        return source;
    }
}

public static class ArrayExtensions
{
    public static int[] Fill(this int[] range)
    {
        var min = range.Min();
        return Enumerable.Range(min, range.Max() - min + 1).ToArray();
    }
}

public static class PointExtensions
{
    public static Point[] Till(this Point from, Point to)
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