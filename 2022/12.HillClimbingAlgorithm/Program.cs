using System.Drawing;

namespace HillClimbingAlgorithm;

public abstract class Program
{
    record Cell(Point Point, char Value)
    {
        public char Value { get; set; } = Value;
    }

    record Bookmark(Cell Start, Cell Destination);

    record Path(List<Cell> Steps)
    {
        public bool? IsSuccess { get; set; }
    }

    public static void Main()
    {
        var inputs = File.ReadAllLines("inputs-demo.txt")
            .Select(s => s.Replace('S', 'z').ToCharArray()).ToArray();

        Cell start = new(Find(inputs, 'z'), 'z'),
            destination = new(Find(inputs, 'E'), 'E');

        var paths = new List<Path> { new(new List<Cell>() { start }) };

        GetClimbPaths(inputs, new(start, destination), ref paths);

        Console.WriteLine($"Part One : {paths.Where(w => w.IsSuccess ?? false).Min(x => x.Steps.Count)}");
    }

    private static void GetClimbPaths(char[][] inputs, Bookmark bookmark, ref List<Path> paths)
    {
        var path = paths.First(f => f.IsSuccess == null);
        var from = path.Steps.Last();
        var directions = GetDirections(inputs, from.Point)
            .Where(w => path.Steps.All(a => a.Point != w.Point)).ToArray();

        if (directions.Any(cell => cell.Point == bookmark.Destination.Point))
        {
            path.Steps.Add(bookmark.Destination);
            path.IsSuccess = true;
            return;
        }

        if (directions.Any(x => from.Value + 1 >= x.Value))
        {
            foreach (var direction in directions)
            {
                if (!CheckIfPathIsTravelled(paths, path, direction))
                {
                    path.Steps.Add(direction);
                    GetClimbPaths(inputs, bookmark, ref paths);
                }
            }
        }
        
        path.IsSuccess = false;
        paths.Add(new Path(new List<Cell>() { bookmark.Start }));
    }

    private static bool CheckIfPathIsTravelled(List<Path> paths, Path path, Cell cell)
    {
        return paths.Any(x => x.IsSuccess != null && string.Concat(x.Steps.Select(s => s.Point).Select(s => $"({s.X}{s.Y})"))
            .StartsWith($"{string.Concat(path.Steps.Select(s => s.Point).Select(s => $"({s.X}{s.Y})"))}{cell.Point.X}{cell.Point.Y}"));
    }

    private static Cell[] GetDirections(char[][] inputs, Point point)
    {
        var cells = new List<Cell>(); // NEWS
        if (point.Y > 0) cells.Add(new(point with { Y = point.Y - 1 }, inputs[point.Y - 1][point.X]));
        if (point.X < inputs.Length - 1) cells.Add(new(point with { X = point.X + 1 }, inputs[point.Y][point.X + 1]));
        if (point.X > 0) cells.Add(new(point with { X = point.X - 1 }, inputs[point.Y][point.X - 1]));
        if (point.Y < inputs[0].Length - 1)
            cells.Add(new(point with { Y = point.Y + 1 }, inputs[point.Y + 1][point.X]));
        return cells.ToArray();
    }

    private static Point Find(char[][] inputs, char find)
    {
        for (var y = 0; y < inputs.Length; y++)
            for (var x = 0; x < inputs[y].Length; x++)
                if (inputs[y][x] == find) return new Point(x, y);

        throw new KeyNotFoundException(find.ToString());
    }
}