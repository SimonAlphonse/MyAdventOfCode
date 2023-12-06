using Extensions;
using System.Collections.Concurrent;
using System.Numerics;
using System.Linq;

var lines = File.ReadAllText("inputs.txt")
    .Split($"{Environment.NewLine}{Environment.NewLine}").ToArray();

var seeds = lines[0][7..].Split(' ').Select(s => long.Parse(s)).ToArray();

var locations = seeds.AsParallel().Select(s => GetSeedLocation(s, lines)).ToArray();

Console.WriteLine($"Part One : {locations.Min()}");

locations = GetMoreSeedsLocations(seeds, lines).ToArray();

Console.WriteLine($"Part Two : {locations.Min()}");

Console.Read();

T GetSeedLocation<T>(T seed, string[] lines) where T : INumber<T>
{
    var cache = seed;

    foreach (var line in lines[1..].Select(s => s.Split(':').Last()))
    {
        var maps = line.Trim().Split(Environment.NewLine)
            .Select(s => s.Split(' ').Select(s => T.Parse(s, default)).ToArray())
            .Select(s => new Map<T>(s[1], s[0], s[2])).ToArray();

        cache = GetLocation(maps, cache);
    }

    //Console.WriteLine($"{seed} -> {cache}");

    return cache;
}

IEnumerable<T> GetMoreSeedsLocations<T>(T[] seeds, string[] lines) where T : INumber<T>
{
    ConcurrentBag<T> result = [];

    Parallel.ForEach(seeds.Chunk(2), chunk =>
    //foreach (var chunk in seeds.Chunk(2))
    {
        var locations = GenericExtensions.Range(chunk.First(), chunk.Last())
            .AsParallel().Select(seed => GetSeedLocation(seed, lines)).ToArray();

        locations.AsParallel().ForAll(result.Add);
    });

    return result;
}

T GetLocation<T>(Map<T>[] maps, T seed) where T : INumber<T>
{
    foreach (var map in maps.Where(map => map.Source <= seed && seed < map.Source + map.Range))
        return map.Destination + seed - map.Source;

    return seed;
}

record Map<T>(T Source, T Destination, T Range) where T : INumber<T>;

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