using System.Drawing;
using System.Collections.Concurrent;

namespace PyroclasticFlow;

internal abstract class Program
{
    public static void Main()
    {
        var wind = File.ReadAllText("inputs-demo.txt")
            .Select(s => s == '<' ? -1 : 1).ToArray();

        var shapes = new[]
        {
            new[,] { { '#', '#', '#', '#' } }.ToJagged(),
            new[,] { { '.', '#', '.' }, { '#', '#', '#' }, { '.', '#', '.' }, }.ToJagged(),
            new[,] { { '.', '.', '#' }, { '.', '.', '#' }, { '#', '#', '#' }, }.ToJagged(),
            new[,] { { '#' }, { '#' }, { '#' }, { '#' }, }.ToJagged(),
            new[,] { { '#', '#' }, { '#', '#' }, }.ToJagged(),
        };

        const int gap = 3, width = 7;
        var points = shapes.Select(s => s.FindPoints('#')).ToArray();
        int wave = 0, height = shapes.Max(s => s.GetLength(0)) * 2022;
        Point start = new(height - gap - shapes[0].Length, 2), position = start;
        var space = '.'.ToArray(height, width);

        for (var rock = 0; rock < 2022;)
        {
            var shapeNo = rock % shapes.Length;
            position = Foo(space, shapes[shapeNo], position, wind[wave++ % wind.Length]);

            if (IsDeadEnd(space, points[shapeNo].Last(), position.OffsetRowBy(1)))
            {
                space.DrawShape(points[shapeNo], position, '#');
                space.Export("PartOne.txt"); Console.WriteLine();
                position = new(space.Find('#').X - gap - shapes[++rock % shapes.Length].Length, 2);
            }
            else
                position = position.OffsetRowBy(1);
        }

        Console.ReadKey();
    }

    private static Point Foo(char[][] space, Point[][] points, Point position, int force)
    {
        var height = points.Length;
        var width = points.Max(x => x.Length);

        var isWall = force switch
        {
            // 1 => IsDeadEnd(space,points.SelectMany(x=>x)),
            _ => false
        };
        
        var point = position.OffsetColumnBy(force);
        var offset = 6 - (point.Y + width - 1);
        if (offset < 0) point = point.OffsetColumnBy(offset);
        Console.WriteLine($"{point.Y}");
        return point;
    }

    private static bool IsDeadEnd(char[][] space, IEnumerable<Point> points, Point position)
    {
        return points.Select(s => s.OffsetBy(position))
            .Select(s => s.X < space.Length ? space[s.X][s.Y] : '#').Contains('#');
    }
}

public static class PointExtensions
{
    public static Point OffsetBy(this Point a, Point b)
    {
        return a with { X = a.X + b.X, Y = a.Y + b.Y };
    }

    public static Point OffsetRowBy(this Point point, int offset)
    {
        return point with { X = point.X + offset  };
    }
    
    public static Point OffsetColumnBy(this Point point, int offset)
    {
        return point with { Y = point.Y + offset };
    }
}

public static class ArrayExtensions
{
    public static Point Find<T>(this T[][] array, T value) where T : struct
    {
        var point = Point.Empty;

        Parallel.For(0, array.Length, (x, state) =>
        {
            for (var y = 0; y < array[x].Length; y++)
            {
                if (array[x][y].Equals(value))
                {
                    point = new Point(x, y);
                    state.Stop();
                }
            }
        });

        return point;
    }

    public static Point[][] FindPoints<T>(this T[][] array, T value) where T : struct
    {
        ConcurrentBag<Point> points = new();

        Parallel.For(0, array.Length, x =>
        {
            Parallel.For(0, array[x].Length, y =>
            {
                if (array[x][y].Equals(value))
                    points.Add(new(x, y));
            });
        });

        return points.OrderBy(o => o.X).ThenBy(o => o.Y).GroupBy(point => point.X)
            .Select(g => g.ToArray()).ToArray();
    }

    public static T[][] DrawShape<T>(this T[][] space, Point[][] shape, Point point, T @char)
    {
        foreach (var item in shape.SelectMany(s => s))
            space[item.X + point.X][item.Y + point.Y] = @char;

        return space;
    }
    
    public static T[][] Replace<T>(this T[][] array, T find, T replace) where T : struct
    {
        Parallel.For(0, array.Length, x =>
        {
            Parallel.For(0, array[x].Length, y =>
            {
                if (array[x][y].Equals(find))
                    array[x][y] = replace;
            });
        });
        
        return array;
    }

    public static void Export<T>(this T[][] array, string fileName, bool append = false)
    {
        using StreamWriter file = new(fileName, append);

        foreach (var item in array.Select((row, index) => (row, index)))
        {
            foreach (var field in item.row)
            {
                file.Write($"{field}");
            }

            if (item.index < array.Length - 1)
                file.Write(Environment.NewLine);
        }
    }

    public static T[][] ToArray<T>(this T value, int rows, int columns) where T : struct
    {
        return Enumerable.Range(0, rows).Select(x =>
            Enumerable.Range(0, columns).Select(y => value).ToArray()).ToArray();
    }

    public static T[][] ToJagged<T>(this T[,] array)
    {
        return Enumerable.Range(0, array.GetLength(0)).Select(x =>
            Enumerable.Range(0, array.GetLength(1)).Select(y => array[x, y]).ToArray()).ToArray();
    }

    public static T[] GetColumn<T>(this T[][] array, int column)
    {
        return array.Select(s => s.Skip(column).First()).ToArray();
    }
    
    public static T[] GetLastColumn<T>(this T[][] array)
    {
        return array.Select(s => s.Last()).ToArray();
    }
}