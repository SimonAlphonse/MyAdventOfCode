using System;
using System.Linq;
using System.Numerics;

var lines = File.ReadAllText("sample.txt")
    .Split($"{Environment.NewLine}{Environment.NewLine}");

var seeds = lines[0][7..].Split(' ').Select(int.Parse);
var maps = lines[1..].Select(s => s.Split(':').Last()).Select(s =>
    s.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s.Split(' ').Select(int.Parse).ToArray()).ToArray());

var lookup = maps.Select(s => s.SelectMany(s => Enumerable.Range(0, s[2])
            .ToDictionary(i => s[1] + i, i => s[0] + i)).ToDictionary()).ToArray();

List<int> locations = [];

int cache = 0;

foreach (var seed in seeds)
{
    cache = seed;

    foreach (var map in lookup)
    {
        if (map.ContainsKey(cache))
        {
            cache = map[cache];
        }
    }

    locations.Add(cache);
}

Console.WriteLine($"Part One : {locations.Min()}");

//Console.WriteLine($"Part Two : {}");

Console.Read();

namespace Extensions
{
    public static class Enumerable
    {
        public static IEnumerable<T> Range<T>(T start, T range) where T : INumber<T>
        {
            for (var i = T.Zero; start < range; i++)
                yield return start + i;
        }
    }
}