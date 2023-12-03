using System.Drawing;
using System.Globalization;
using System.Linq;
using Extensions;

internal class Program
{
    private static void Main(string[] args)
    {
        var file = "inputs.txt";

        char[] numbers = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];
        char[] symbols = File.ReadAllText(file)
            .Where(w => !char.IsNumber(w))
            .Except(['.', '\n', '\r'])
            .Distinct().ToArray();

        var lines = File.ReadLines(file).ToArray();
        Size size = new(lines.First().Length, lines.Length);
        int sum1 = 0;

        for (int y = 0; y < size.Width; y++)
        {
            var indices = lines[y].Select((_, index) => index)
                .Where(index => char.IsNumber(lines[y][index])).ToArray();

            sum1 += indices.SplitConsecutive()
                .Where(s => symbols.Intersect(
                    s.SelectMany(s => new Point(y, s)
                        .GetAdjacentPoints(size)
                        .Select(w => lines[w.X][w.Y]))
                    ).Any()).Select(s => int.Parse(string.Join(string.Empty, s.Select(s => lines[y][s])))).Sum();
        }

        Console.WriteLine($"Part One : {sum1}");


        //Console.WriteLine($"Part Two : {sum2}");

        Console.Read();
    }
}

namespace Extensions
{
    public static class IntExtensions
    {
        public static IEnumerable<IEnumerable<int>> SplitConsecutive(this IEnumerable<int> values)
        {
            return values.SplitConsecutiveIndices().Select(a => a.Select(b => values.Skip(b).First()));
        }

        public static IEnumerable<IEnumerable<int>> SplitConsecutiveIndices(this IEnumerable<int> indices)
        {
            return indices.Select((_, index) => index)
                .GroupBy(index => indices.Skip(index).First() - index)
                .Select(group => group.ToArray()).ToArray();
        }
    }

    public static class PointExtensions
    {
        public static Point OffsetBy(this Point point, Point offset)
        {
            point.Offset(offset);
            return point;
        }

        public static Point[] GetAdjacentPoints(this Point center, Size size)
        {
            return new List<Point>() {
                    center.OffsetBy(new(-1, -1)),
                    center.OffsetBy(new(0, -1)),
                    center.OffsetBy(new(1, -1)),
                    center.OffsetBy(new(-1, 0)),
                    center.OffsetBy(new(1, 0)),
                    center.OffsetBy(new(-1, 1)),
                    center.OffsetBy(new(0, 1)),
                    center.OffsetBy(new(1, 1))
                }.Where(w => w.X >= 0 && w.X < size.Width &&
                             w.Y >= 0 && w.Y < size.Height).ToArray();
        }

        public static Point[] GetAdjacentPoints(this IEnumerable<Point> points, Size size)
        {
            return points.SelectMany(s => s.GetAdjacentPoints(size)).Except(points).Distinct().ToArray();
        }
    }
}