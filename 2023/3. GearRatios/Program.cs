using Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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

        PartInfo[] partsInfo = GetPartInfo(lines, size);

        int sum1 = partsInfo.Where(w => w.AdjacentPoints.Values.Intersect(symbols).Any()).Sum(s => s.Value);

        Console.WriteLine($"Part One : {sum1}");

        var partsWithGear = partsInfo.Where(w => w.AdjacentPoints.ContainsValue('*'))
            .Select(s => (s.Value, Point: s.AdjacentPoints.Where(w => w.Value == '*').Select(s => s.Key).First()));

        int sum2 = partsWithGear.GroupBy(g => g.Point).Where(w => w.Count() == 2).Sum(s => s.Aggregate(1, (a, b) => a * b.Value));

        Console.WriteLine($"Part Two : {sum2}");

        Console.Read();
    }

    private static PartInfo[] GetPartInfo(string[] lines, Size size)
    {
        List<PartInfo> partsInfo = [];

        for (int y = 0; y < size.Width; y++)
        {
            var indices = lines[y].GetNumberIndices();

            partsInfo.AddRange(indices.SplitConsecutive().Select(s => s.Select(s => new Point(y, s)))
               .Select(s => (
                    Value: int.Parse(string.Join(string.Empty, s.Select(s => lines[s.X][s.Y]))),
                    AdjacentPoints: s.GetAdjacentPoints(size).ToDictionary(key => key, d => lines[d.X][d.Y])))
               .Select(s => new PartInfo(s.Value, s.AdjacentPoints)));
        }

        return partsInfo.ToArray();
    }

    internal record struct PartInfo(int Value, Dictionary<Point, char> AdjacentPoints);
}


namespace Extensions
{
    public static class StringExtensions
    {
        public static int[] GetNumberIndices(this string line)
        {
            return line.Select((_, index) => index).Where(index => char.IsNumber(line[index])).ToArray();
        }
    }

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

