using Extensions;
using System.Numerics;

var lines = File.ReadAllText("sample.txt")
    .Split($"{Environment.NewLine}{Environment.NewLine}");

var lookup = GetLookup<BigInteger>(lines[1..]).ToArray();
var locations = GetLocations(lines[0], lookup);

Console.WriteLine($"Part One : {locations.Min()}");

//Console.WriteLine($"Part Two : {}");

Console.Read();

IEnumerable<T> GetLocations<T>(string seeds, Dictionary<T, T>[] lookup) where T : INumber<T>
{
    foreach (var seed in seeds[7..].Split(' ').Select(s => T.Parse(s, default)))
    {
        T cache = seed;

        foreach (var map in lookup)
            if (map.TryGetValue(cache, out T value))
                cache = value;

        yield return cache;
    };
}

IEnumerable<Dictionary<T, T>> GetLookup<T>(IEnumerable<string> lines) where T:INumber<T>
{
    foreach (var line in lines.Select(s => s.Split(':').Last()))
    {
        yield return line.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Split(' ').Select(s => T.Parse(s, default)).ToArray())
            .SelectMany(s => GenericExtensions.Range<T>(T.Zero, s[2])
            .ToDictionary(i => s[1] + i, i => s[0] + i)).ToDictionary();
    }
}

namespace Extensions
{
    public static class GenericExtensions
    {
        public static IEnumerable<T> Range<T>(T start, T range) where T : INumber<T>
        {
            for (var i = T.Zero + start; i < start + range; i++)
                yield return start + i;
        }
    }
}