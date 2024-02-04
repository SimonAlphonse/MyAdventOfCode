using Extensions;
using System.Numerics;

internal class Program
{
    private static void Main(string[] args)
    {
        var inputs = File.ReadAllLines("inputs.txt")
            .Select(s => s.Split(':').Last()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse).ToArray()).ToArray();

        var races = GenericExtensions.Range<long>(0, inputs[0].Length)
            .Select(i => new Race<long>(Time: inputs[0][i], Distance: inputs[1][i])).ToArray();

        Console.WriteLine($"Part One : {GetRecordCounts(races).Aggregate((a, b) => a * b)}");

        Race<long> race = races.Aggregate((x, y) => new(
            long.Parse($"{x.Time}{y.Time}"),
            long.Parse($"{x.Distance}{y.Distance}")));

        Console.WriteLine($"Part Two : {GetRecordCount(race, 14)}");

        Console.Read();
    }

    private static IEnumerable<int> GetRecordCounts<T>(Race<T>[] races) where T:INumber<T>
    {
        foreach (var race in races)
            yield return GetRecordCount(race, T.Zero);
    }

    private static int GetRecordCount<T>(Race<T> race, T start) where T : INumber<T>
    {
        return GenericExtensions.Range(start, race.Time + T.One).AsParallel()
            .Select(hold => hold * (race.Time - hold)).Count(w => w > race.Distance);
    }
}

internal record struct Race<T>(T Time, T Distance) where T : INumber<T>;

namespace Extensions
{
    public static class GenericExtensions
    {
        public static IEnumerable<T> Range<T>(T start, T range) where T : INumber<T>
        {
            for (var i = T.Zero; i < range; i++)
                yield return start + i;
        }
    }
}