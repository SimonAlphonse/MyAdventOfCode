using System.Drawing;

namespace BeaconExclusionZone;

internal abstract class Program
{
    record Spot(Point Sensor, Point Beacon)
    {
        public Point Beacon { get; set; } = Beacon;
        public Point Sensor { get; set; } = Sensor;
    }

    record Offset(int X, int Y)
    {
        public int X { get; set; } = X;
        public int Y { get; set; } = Y;
    }

    public static void Main()
    {
        var spots = File.ReadAllLines("inputs.txt")
            .Select(a => a.Split(' '))
            .Select(b => new Spot(
                new(int.Parse(b[3][2..^1]),int.Parse(b[2][2..^1])),
                new(int.Parse(b[9][2..]),int.Parse(b[8][2..^1])))).ToArray();
        
        var offset = CreateSpace(spots, out var space);
        space.Fill('.');
        OffsetSpots(spots, offset);
        space = MarkSpots(space, spots.Select(s => s.Sensor).ToArray(), 'S');
        space = MarkSpots(space, spots.Select(s => s.Beacon).ToArray(), 'B');
        space.Export("PartOne.txt");
    }
    
    private static Offset CreateSpace(Spot[] spots, out char[,] space)
    {
        var xMin = spots.Select(spot => Math.Min(spot.Sensor.X, spot.Beacon.X)).Min();
        var xMax = spots.Select(spot => Math.Max(spot.Sensor.X, spot.Beacon.X)).Max();
        var yMin = spots.Select(spot => Math.Min(spot.Sensor.Y, spot.Beacon.Y)).Min();
        var yMax = spots.Select(spot => Math.Max(spot.Sensor.Y, spot.Beacon.Y)).Max();

        space = new char[xMax + Math.Abs(xMin) + 1, yMax + Math.Abs(yMin) + 1];

        return new(xMin, yMin);
    }

    private static void OffsetSpots(Spot[] spots, Offset offset)
    {
        foreach (var spot in spots)
        {
            int x = Math.Abs(offset.X), y = Math.Abs(offset.Y);
            
            switch (Math.Sign(offset.X), Math.Sign(offset.Y))
            {
                case (-1, -1):
                    spot.Sensor = spot.Sensor with { X = spot.Sensor.X + x, Y = spot.Sensor.Y + y };
                    spot.Beacon = spot.Beacon with { X = spot.Beacon.X + x, Y = spot.Beacon.Y + y };
                    break;
                case (-1, _):
                    spot.Sensor = spot.Sensor with { X = spot.Sensor.X + x };
                    spot.Beacon = spot.Beacon with { X = spot.Beacon.X + x };
                    break;
                case (_, -1):
                    spot.Sensor = spot.Sensor with { Y = spot.Sensor.Y + y };
                    spot.Beacon = spot.Beacon with { Y = spot.Beacon.Y + y };
                    break;
            }
        }
    }

    private static char[,] MarkSpots(char[,] space, Point[] points, char marker)
    {
        foreach (var point in points)
        {
            space[point.X, point.Y] = marker;
        }
        
        return space;
    }
}

public static class ArrayExtensions
{
    public static T[,] Fill<T>(this T[,] array, T value)
    {
        for (var i = 0; i < array.GetLength(0); i++)
        {
            for (var j = 0; j < array.GetLength(1); j++)
            {
                array[i, j] = value;
            }
        }

        return array;
    }

    public static void Export<T>(this T[,] drawing, string fileName)
    {
        File.Delete(fileName);

        using StreamWriter file = new(fileName, append: true);

        for (var i = 0; i < drawing.GetLength(0); i++)
        {
            for (var j = 0; j < drawing.GetLength(1); j++)
            {
                file.Write($"{drawing[i, j]}");
            }

            file.Write(Environment.NewLine);
        }
    }
}