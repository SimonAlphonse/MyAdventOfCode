namespace RucksackReorganization;

public abstract class Program
{
    public static void Main()
    {
        var lines = File.ReadAllLines("inputs.txt");

        var priorities = Enumerable.Range('a', 'z' - 'a' + 1)
            .Concat(Enumerable.Range('A', 'Z' - 'A' + 1))
            .Select(s => (char)s).ToList();

        var markers = lines.Select(line => line.Chunk(line.Length / 2)).Select(s => s.Intersect().First());
        Console.WriteLine($"Part One : {markers.Sum(s => priorities.IndexOf(s) + 1)}");

        var badges = lines.Chunk(3).Select(s => s.Intersect().First());
        Console.WriteLine($"Part Two : {badges.Sum(s => priorities.IndexOf(s) + 1)}");

        Console.ReadKey();
    }
}

public static class Extensions
{
    public static IEnumerable<T> Intersect<T>(this IEnumerable<IEnumerable<T>> collections)
    {
        if (collections.Count() < 2) { return Array.Empty<T>(); }

        IEnumerable<T> common = collections.First().Intersect(collections.Skip(1).First());

        foreach (var collection in collections.Skip(2))
            common = common.Intersect(collection);

        return common;
    }
}