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
        public bool IsSuccess { get; set; }
        public List<Cell> Travelled { get; set; } = new();
    }

    public static void Main()
    {
        var inputs = File.ReadAllLines("inputs-demo.txt").Select(s=> s.ToCharArray()).ToArray();

        Cell start = new(Find(inputs, 'S'), 'a'), 
            destination = new(Find(inputs, 'E'), 'z');
        
        inputs[start.Point.Y][start.Point.X] = 'z';
        inputs[destination.Point.Y][destination.Point.X] = 'z';

        var paths = new List<Path> { new(new List<Cell>() { start }) };

        GetClimbPaths(inputs, new(start, destination), ref paths);

        Console.WriteLine($"Part One : {paths.Where(w => w.IsSuccess).Min(x => x.Steps.Count)}");
    }

    private static void GetClimbPaths(char[][] inputs, Bookmark bookmark, ref List<Path> paths)
    {
        var path = paths.First(f => !f.IsSuccess);
        var from = path.Steps.Last();
        var directions = GetDirections(inputs, from)
            .Where(w => path.Steps.All(a => a.Point != w.Point) && 
                        path.Travelled.All(a => a.Point != w.Point) &&
                        from.Value + 1 >= w.Value).ToArray();

        if (directions.Any(cell => cell.Point == bookmark.Destination.Point))
        {
            path.Steps.Add(bookmark.Destination);
            path.IsSuccess = true;
            
            paths.Add(new Path(new List<Cell>() { bookmark.Start }));
            GetClimbPaths(inputs, bookmark, ref paths);
        }

        if (directions.Any())
        {
            foreach (var direction in directions)
            {
                path.Steps.Add(direction);
                GetClimbPaths(inputs, bookmark, ref paths);
            }
        }
        else
        {
            path.Steps.Remove(from);
            path.Travelled.Add(from);
            GetClimbPaths(inputs, bookmark, ref paths);
        }
    }

    private static Cell[] GetDirections(char[][] inputs, Cell cell)
    {
        var cells = new List<Cell>(); // NEWS
        if (cell.Point.Y > 0) /* ^ */
            cells.Add(new(cell.Point with { Y = cell.Point.Y - 1 }, inputs[cell.Point.Y - 1][cell.Point.X]));
        if (cell.Point.X < inputs[0].Length - 1) /* > */
            cells.Add(new(cell.Point with { X = cell.Point.X + 1 }, inputs[cell.Point.Y][cell.Point.X + 1]));
        if (cell.Point.X > 0) /* < */
            cells.Add(new(cell.Point with { X = cell.Point.X - 1 }, inputs[cell.Point.Y][cell.Point.X - 1]));
        if (cell.Point.Y < inputs.Length - 1) /* V */
            cells.Add(new(cell.Point with { Y = cell.Point.Y + 1 }, inputs[cell.Point.Y + 1][cell.Point.X]));
        return cells.ToArray();
    }

    private static Point Find(char[][] inputs, char find)
    {
        for (var y = 0; y < inputs.Length; y++)
            for (var x = 0; x < inputs[y].Length; x++)
                if (inputs[y][x] == find)
                    return new(x, y);

        throw new KeyNotFoundException(find.ToString());
    }
}