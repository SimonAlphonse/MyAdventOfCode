using System.Drawing;

namespace PyroclasticFlow;

internal abstract class Program
{
    record Line(char[] Characters, Point Point);

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

        int x = 2, y = 3, index = 0, rocks = 2022,
            height = shapes.Max(s => s.GetLength(0)) * rocks;

        Point start = new(x, y), position = new(x, y);
        var space = CreateSpace(height, '.');

        for (var rock = 0; rock < rocks; rock++)
        {
            var bottom = space.Find('#');
            int floor = bottom.IsEmpty ? -1 : bottom.X;
            var shape = shapes[rock % shapes.Length];
            position = position with { X = x + floor, Y = position.Y + Math.Sign(wind[index++]) };

            var @base = Enumerable.Range(position.Y, shape.GetLength(1))
                .Select(x => space[position.X - shape.Length, x]).ToArray();
        }
    }

    private static bool IsOverlap(IEnumerable<Line> shapes)
    {

        return default;
    }

    private static char[,] CreateSpace(int height, char @char)
    {
        var space = new char[7, height];

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
    public static Point Find<T>(this T[,] array, T value) where T : struct
    {
        var point = Point.Empty;

        Parallel.For(0, array.GetLength(0),
            (index, state) =>
            {
                for (var y = 0; y < array.GetLength(1); y++)
                {
                    if (array[index, y].Equals(value))
                    {
                        point = new Point(index, y);
                        state.Stop();
                    }
                }
            });

        return point;
    }
}