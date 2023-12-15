using Extensions;
using System.Numerics;
using System.Collections.Concurrent;
using System.Drawing;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using static Extensions.PointExtensions;

var lines = File.ReadAllText("sample.txt")
    .Split($"{Environment.NewLine}{Environment.NewLine}").ToArray();

var maps = lines[1..].Select(s => s.Split(':').Last())
    .Select(s => s.Trim().Split(Environment.NewLine)
        .Select(s => s.Split(' ').Select(s => long.Parse(s)).ToArray())
        .Select(s => new Map<long>(s[1], s[0], s[2])).ToArray()).ToArray();

var seeds = lines[0][7..].Split(' ').Select(s => long.Parse(s)).ToArray();

var locations = seeds.AsParallel().Select(s => GetSeedLocation(s, lines)).ToArray();

Console.WriteLine($"Part One : {locations.Min()}");

var map = FlattenMaps(maps).ToArray();

locations = GetMoreSeedsLocations(seeds, lines).ToArray();

Console.WriteLine($"Part Two : {locations.Min()}");

Console.Read();

Map<T>[] FlattenMaps<T>(Map<T>[][] maps) where T : INumber<T>
{
    List<Map<T>> result = [];

    for (int i = 1; i < maps.Length - 1; i++)
    {
        var from = maps.First();
        var to = maps.Skip(i).First();

        foreach (var first in from)
        {
            foreach (var second in to)
            {
                _ = Map<T>.GetOverlapType(first, second) switch
                {
                    OverlapType.SecondOverlapsFirst => "",
                    _ => ""
                };
            }
        }
    }

    return result.ToArray();
}

T GetSeedLocation<T>(T seed, string[] lines) where T : INumber<T>
{
    var cache = seed;

    foreach (var line in lines[1..].Select(s => s.Split(':').Last()))
    {
        var maps = line.Trim().Split(Environment.NewLine)
            .Select(s => s.Split(' ').Select(s => T.Parse(s, default)).ToArray())
            .Select(s => new Map<T>(s[1], s[0], s[2])).ToArray();

        //cache = GetLocation(maps, cache);
    }

    return cache;
}

IEnumerable<T> GetMoreSeedsLocations<T>(T[] seeds, string[] lines) where T : INumber<T>
{
    ConcurrentBag<T> result = [];

    Parallel.ForEach(seeds.Chunk(2), chunk =>
    {
        var locations = GenericExtensions.Range(chunk.First(), chunk.Last())
            .AsParallel().Select(seed => GetSeedLocation(seed, lines)).ToArray();

        locations.AsParallel().ForAll(result.Add);
    });

    return result;
}

//T GetLocation<T>(Map<T>[] maps, T seed) where T : INumber<T>
//{
//    foreach (var map in maps.Where(map => map.Source <= seed && seed < map.Source + map.Range))
//        return map.Destination + seed - map.Source;

//    return seed;
//}

class Map<T>() where T : INumber<T>
{
    public Location<T> Source { get; }
    public Location<T> Destination { get; }
    public T Offset { get; }
    public T Range { get; }

    public Map(T source, T destination, T range) : this()
    {
        this.Source = new(source, source + range - T.One);
        this.Destination = new(destination, destination + range - T.One);
        this.Offset = destination - source;
        this.Range = range;
    }

    public static OverlapType GetOverlapType(Map<T> first, Map<T> second)
    {
        return (first.Destination, second.Source) switch
        {
            var (one, two) when one.Start < two.Start && one.End >= two.Start => OverlapType.PartiallyAtEnd,
            var (one, two) when one.Start > two.Start && one.End <= two.Start => OverlapType.PartiallyAtStart,
            var (one, two) when one.Start <= two.Start && one.End >= two.End => OverlapType.FirstOverlapsSecond,
            var (one, two) when two.Start <= one.Start && two.End >= one.End => OverlapType.SecondOverlapsFirst,
            _ => OverlapType.None,
        };
    }
}

record Location<T>(T Start, T End) where T : INumber<T>;

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

    public static class PointExtensions
    {
        public enum OverlapType
        {
            None = 0,
            PartiallyAtEnd = 1,
            PartiallyAtStart = 2,
            FirstOverlapsSecond = 3,
            SecondOverlapsFirst = 4,
        }

        public static OverlapType GetOverlapType(this Point first, Point second)
        {
            return (first, second) switch
            {
                var (one, two) when one.X < two.X && one.Y >= two.X => OverlapType.PartiallyAtEnd,
                var (one, two) when one.X > two.X && one.Y <= two.X => OverlapType.PartiallyAtStart,
                var (one, two) when one.X <= two.X && one.Y >= two.Y => OverlapType.FirstOverlapsSecond,
                var (one, two) when two.X <= one.X && two.Y >= one.Y => OverlapType.SecondOverlapsFirst,
                _ => OverlapType.None,
            };
        }
    }
}