using System.Drawing;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.InteropServices.ComTypes;
using System.Xml;

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

        var points = shapes.Select(s => s.FindPoints('#')).ToArray();
        int wave = 0, gap = 3, width = 7, height = shapes.Max(s => s.GetLength(0)) * 2022;
        Point start = new(height - gap, 2), position = start.OffsetRowBy(shapes[0].Length - 1);
        var space = '.'.ToArray(height, width, x => '.');

        for (var rock = 0; rock < 2022;)
        {
            var shapeNo = rock % shapes.Length;

            position = ChangePositionByWind(position, wind, wave++, shapes[shapeNo].Max(x => x.Length));

            var @base = GetBase(space, points[shapeNo].Last(), position);

            if (@base.Contains('#'))
            {
                space.DrawShape(points[shapeNo], position.OffsetRowBy(-1), '#');
                var floor = space.Find('#', out var temp) ? temp.X : throw new WarningException("🤬");
                position = position with { X = floor - gap - shapes[0].Length + 1, Y = 2 };
                rock++;
            }
            else
            {
                position = position.OffsetRowBy(1);
            }

            space.Export("PartOne.txt");
        }

        Console.ReadKey();
    }

    private static char[] GetBase(char[][] space, Point[] points, Point position)
    {
        return points.Select(s => s.OffsetBy(position))
            .Select(s =>
                s.X < space.Length
                    ? space[s.X][s.Y]
                    : '#').ToArray();
    }

    private static Point ChangePositionByWind(Point position, int[] wind, int index, int width)
    {
        var point = position.OffsetColumnBy(GetWindForce(wind, index));
        var offset = 6 - (point.Y + (width - 1));
        if (offset < 0) point = point.OffsetColumnBy(offset);
        return point;
    }

    private static int GetWindForce(int[] wind, int index)
    {
        return Math.Sign(wind[index % wind.Length]);
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
    public static bool Find<T>(this T[][] array, T value, out Point position) where T : struct
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

        position = point;

        return !position.IsEmpty;
    }

    public static Point[][] FindPoints<T>(this T[][] array, T value) where T : struct
    {
        ConcurrentBag<Point> points = new();

        Parallel.For(0, array.Length, x =>
        {
            Parallel.For(0, array[x].Length, y =>
            {
                if (array[x][y].Equals(value))
                {
                    points.Add(new(x, y));
                }
            });
        });

        return points.GroupBy(point => point.X).OrderBy(o => o.Key)
            .Select(g => g.OrderBy(o => o.X).ThenBy(o => o.Y).ToArray()).ToArray();
    }

    public static char[][] DrawShape(this char[][] space, Point[][] shape, Point point, char @char)
    {
        foreach (var item in shape.SelectMany(s => s))
        {
            space[item.X + point.X][item.Y + point.Y] = @char;
        }

        return space;
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

    public static T[][] ToArray<T>(this T @char, int rows, int columns, Func<int, T> selector) where T : struct
    {
        return Enumerable.Range(0, rows).Select(x =>
            Enumerable.Range(0, columns).Select(selector).ToArray()).ToArray();
    }

    public static T[][] ToJagged<T>(this T[,] array)
    {
        return Enumerable.Range(0, array.GetLength(0)).Select(x =>
            Enumerable.Range(0, array.GetLength(1)).Select(y => array[x, y]).ToArray()).ToArray();
    }
}