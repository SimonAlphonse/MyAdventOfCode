using System.Collections.Concurrent;
using System.Drawing;

namespace PyroclasticFlow;

internal abstract class Program
{
    record Line(char[] Characters, Point Point);
    record Dimension(int Height, int Width);

    public static void Main()
    {
        var wind = File.ReadAllText("inputs-demo.txt")
            .Select(s => s == '<' ? -1 : 1).ToArray();

        var shapes = new[]
        {
            new[,]
            {
                { '#', '#', '#', '#' }
            },
            new[,]
            {
                { '.', '#', '.' },
                { '#', '#', '#' },
                { '.', '#', '.' },
            },
            new[,]
            {
                { '.', '.', '#' },
                { '.', '.', '#' },
                { '#', '#', '#' },
            },
            new[,]
            {
                { '#' },
                { '#' },
                { '#' },
                { '#' },
            },
            new[,]
            {
                { '#', '#' },
                { '#', '#' },
            },
        };

        int wave = 0, rocks = 2022;
        int width = 7, height = shapes.Max(s => s.GetLength(0)) * rocks;
        var space = CreateSpace(new(width, height), '.');

        Point gap = new(3, 2), position = gap with { X = height - gap.X };

        for (var rock = 0; rock < rocks;)
        {
            position = ChangePositionByWind(position, space, wind, wave++);

            var floor = GetFloorIndex(space,'#');
            var shape = shapes[rock % shapes.Length];

            var @base = Enumerable.Range(position.Y, shape.GetLength(1))
                    .Select(x => space[position.X + shape.GetLength(0) + 1, x]).ToArray();
            
            //floor > height ? new[] { '.', '.', '.' }
            
            space.Export("PartOne.txt");

            if (IsOverlap(new Line[] { }))
            {
                space = DrawShape(space, shape, position, '#');
                rock++;
            }
            else
            {
                position = position with { X = position.X + 1 };
            }
        }
    }

    private static bool IsOverlap(IEnumerable<Line> shapes)
    {
        
        return default;
    }

    private static int GetFloorIndex(char[,] space, char @char)
    {
        return space.GetLength(0) - (space.Find(@char, out var found) ? found.X : -1);
    }

    private static Point ChangePositionByWind(Point position, char[,] space, int[] wind, int index)
    {
        return position = position with { Y = position.Y + GetWindForce(wind, index) };
    }

    private static int GetWindForce(int[] wind, int index)
    {
        return Math.Sign(wind[index % wind.Length]);
    }

    private static char[,] DrawShape(char[,] space, char[,] shape, Point position, char @char)
    {

        return default;
    }

    private static char[,] CreateSpace(Dimension dimension, char @char)
    {
        var space = new char[dimension.Width, dimension.Height];

        for (var x = 0; x < space.GetLength(0); x++)
        {
            for (var y = 0; y < space.GetLength(1); y++)
            {
                space[x, y] = @char;
            }
        }

        return space;
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
    
    public static bool FindAll<T>(this T[,] array, T value, out Point[] positions) where T : struct
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

        positions = points.ToArray();

        return !points.IsEmpty;
    }
    
    public static void Export<T>(this T[,] array, string fileName)
    {
        File.Delete(fileName);

        using StreamWriter file = new(fileName, append: true);

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