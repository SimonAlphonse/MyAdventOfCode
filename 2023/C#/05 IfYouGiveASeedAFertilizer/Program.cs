using Extensions;
using System.Numerics;

var lines = File.ReadAllText("inputs.txt")
    .Split($"{Environment.NewLine}{Environment.NewLine}").ToArray();

var locations = GetLocations<BigInteger>(lines[0]);

Console.WriteLine($"Part One : {locations.Min()}");

//Console.WriteLine($"Part Two : {}");

Console.Read();

IEnumerable<T> GetLocations<T>(string seeds) where T : INumber<T>
{
    foreach (var seed in seeds[7..].Split(' ').Select(s => T.Parse(s, default)))
    {
        var cache = seed;

        foreach (var line in lines[1..].Select(s => s.Split(':').Last()))
        {
            var split = line.Trim().Split(Environment.NewLine)
                .Select(s => s.Split(' ').Select(s => T.Parse(s, default)).ToArray());

            cache = GetLocation(split.Select(s => new Map<T>(s[1], s[0], s[2])), cache);
        }

        yield return cache;
    }
}

T GetLocation<T>(IEnumerable<Map<T>> maps, T seed) where T : INumber<T>
{
    foreach (var map in maps)
    {
        if (map.Source <= seed && seed < map.Source + map.Range)
            return map.Destination + seed - map.Source;
    }

    return seed;
}

record Map<T>(T Source, T Destination, T Range) where T : INumber<T>;

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