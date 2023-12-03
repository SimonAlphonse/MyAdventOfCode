using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using Extensions;

var file = "sample.txt";

char[] numbers = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];
char[] symbols = File.ReadAllText(file).Distinct().Except([.. numbers, '.', '\n', '\r']).ToArray();

var lines = File.ReadLines(file).ToArray();

Size size = new(lines.First().Length, lines.Length);
char[,] parts = new char[size.Width, size.Height];

List<char> finds = []; List<int> partNumbers = [];

for (int i = 0; i < size.Width * size.Height; i++)
{
    Point center = new(i % size.Width, i / size.Width);

    var value = lines[center.Y][center.X];

    if (int.TryParse(value.ToString(), out _))
    {
        Point[] searchPoints = center.GetAdjacentPoints(size);

        if (!searchPoints.Select(searchPoint => lines[searchPoint.Y][searchPoint.X]).Intersect(symbols).Any())
        {
            if (center.X + 1 == size.Width || !int.TryParse(lines[center.Y][center.X + 1].ToString(), out _))
            {
                if (center.X == 0 || !int.TryParse(lines[center.Y][center.X - 1].ToString(), out _) || finds.Count > 0)
                {
                    finds.Add(value);
                    partNumbers.Add(int.Parse(string.Join(string.Empty, finds)));
                    finds.Clear();
                }
                else { finds.Add(value); continue; }
            }
            else { finds.Add(value); continue; }
        }
        finds.Clear();
    }
    else { finds.Clear(); }
}

Console.WriteLine($"Part One : {partNumbers.Sum()}");


//Console.WriteLine($"Part Two : {sum2}");

Console.Read();



namespace Extensions
{
    public static class PointExtensions
    {
        public static Point OffsetBy(this Point point, Point offset)
        {
            point.Offset(offset);
            return point;
        }

        public static Point[] GetAdjacentPoints(this Point center, Size size)
        {
            var points = new List<Point>() {
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

            return points;
        }

        public static Point[] GetAdjacentPoints(this IEnumerable<Point> points, Size size)
        {
            return points.SelectMany(s => s.GetAdjacentPoints(size)).Except(points).Distinct().ToArray();
        }
    }
}