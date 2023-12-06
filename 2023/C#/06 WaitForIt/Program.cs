using Extensions;
using System.Numerics;

internal class Program
{
    private static void Main(string[] args)
    {
        var inputs = File.ReadAllLines("sample.txt")
            .Select(s => s.Split(':').Last()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse).ToArray()).ToArray();

        var records = GenericExtensions.Range<long>(0, inputs[0].Length)
            .Select(i => new Race<long>(Time: inputs[0][i], Distance: inputs[1][i])).ToArray();

        Console.WriteLine($"Part One : {GetRecordCounts(records).Aggregate((a, b) => a * b)}");

        var race = new Race<long>(long.Parse(string.Join(string.Empty, records.Select(s => s.Time))),
            long.Parse(string.Join(string.Empty, records.Select(s => s.Distance))));

        Console.WriteLine($"Part Two : {GetWinCount(race, 14)}");

        Console.Read();
    }

    private static IEnumerable<int> GetRecordCounts<T>(Race<T>[] races) where T:INumber<T>
    {
        foreach (var race in races)
            yield return GetWinCount(race, T.Zero);
    }

    private static int GetWinCount<T>(Race<T> race, T start) where T : INumber<T>
    {
        return GenericExtensions.Range(start, race.Time + T.One).AsParallel()
            .Select(hold => hold * (race.Time - hold)).Count<T>(w => w > race.Distance);
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