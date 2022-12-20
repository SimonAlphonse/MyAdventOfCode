using System.Collections.Concurrent;
using System.Drawing;

namespace PyroclasticFlow;

internal abstract class Program
{
    public static void Main()
    {
        var wind = File.ReadAllText("inputs-demo.txt")
            .Select(s => s == '<' ? -1 : 1).ToArray();

        var shapes = new[]
        {
            new[,] { { '#', '#', '#', '#' } },
            new[,] { { '.', '#', '.' }, { '#', '#', '#' }, { '.', '#', '.' }, },
            new[,] { { '.', '.', '#' }, { '.', '.', '#' }, { '#', '#', '#' }, },
            new[,] { { '#' }, { '#' }, { '#' }, { '#' }, },
            new[,] { { '#', '#' }, { '#', '#' }, },
        };

        var points = shapes.Select(s => s.FindPoints('#')).ToArray();
        int wave = 0, height = shapes.Max(s => s.GetLength(0)) * 2022;
        Point gap = new(3, 2), start = gap with { X = height - gap.X }, position = start;
        var space = '.'.CreateArray(7, height);
        
        for (var rock = 0; rock < 2022;)
        {
            position = ChangePositionByWind(position, space, wind, wave++);

            var shapeNo = rock % shapes.Length;
            var @base = GetBase(space, shapes[shapeNo], position);
            var shapePoints = points[shapeNo];
            var basePoints = @base.FindPoints('#');

            if (basePoints.Length > 0)
            {
                space = space.DrawShape(shapes[shapeNo], position, '#');
                position = position with { X = position.X - gap.X, Y = 2 };
                rock++;
            }
            else
            {
                position = position with { X = position.X + 1 };
            }

            space.Export("PartOne.txt");
        }
    }

    private static char[,] GetBase(char[,] space, char[,] shape, Point position)
    {
        var @base = new char[1, shape.GetLength(1)]; // todo only consider # on bottom of the row of shape
        for (var column = 0; column < @base.GetLongLength(1); column++)
            @base[0, column] = space[position.X + shape.GetLength(0), column];
        return @base;
    }

    private static Point ChangePositionByWind(Point position, char[,] space, int[] wind, int index)
    {
        return position = position with { Y = position.Y + GetWindForce(wind, index) };
    }

    private static int GetWindForce(int[] wind, int index)
    {
        return Math.Sign(wind[index % wind.Length]);
    }
}

public static class ArrayExtensions
{
    public static bool Find<T>(this T[,] array, T value, out Point position) where T : struct
    {
        var point = Point.Empty;

        Parallel.For(0, array.GetLength(0),
            (x, state) =>
            {
                for (var y = 0; y < array.GetLength(1); y++)
                {
                    if (array[x, y].Equals(value))
                    {
                        point = new Point(x, y);
                        state.Stop();
                    }
                }
            });

        position = point;

        return !position.IsEmpty;
    }
    
    public static Point[] FindPoints<T>(this T[,] array, T value) where T : struct
    {
        ConcurrentBag<Point> points = new();

        Parallel.For(0, array.GetLength(0), x =>
        {
            for (var y = 0; y < array.GetLength(1); y++)
            {
                if (array[x, y].Equals(value))
                {
                    points.Add(new Point(x, y));
                }
            }
        });

        return points.ToArray();
    }
    
    public static char[,] DrawShape(this char[,] space, char[,] shape, Point point, char @char)
    {
        
        return default;
    }
    
    public static char[,] CreateArray(this char @char, int rows, int columns)
    {
        var space = new char[columns, rows];

        for (var x = 0; x < space.GetLength(0); x++)
        {
            for (var y = 0; y < space.GetLength(1); y++)
            {
                space[x, y] = @char;
            }
        }

        return space;
    }
    
    public static void Export<T>(this T[,] array, string fileName, bool append = false)
    {
        using StreamWriter file = new(fileName, append);

        for (var i = 0; i < array.GetLength(0); i++)
        {
            for (var j = 0; j < array.GetLength(1); j++)
            {
                file.Write($"{array[i, j]}");
            }

            file.Write(Environment.NewLine);
        }
    }
}
