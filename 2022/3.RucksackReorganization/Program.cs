namespace RucksackReorganization;

public abstract class Program
{
    public static void Main()
    {
        var lines = File.ReadAllLines("inputs.txt");

        var priorities = Enumerable.Range('a', 'z' - 'a' + 1).Select((s, i) => KeyValuePair.Create((char)s, i + 1))
            .Concat(Enumerable.Range('A', 'Z' - 'A' + 1).Select((s, i) => KeyValuePair.Create((char)s, i + 1 + 26)))
            .ToDictionary(x => x.Key, y => y.Value);

        var markers = lines.Select(line => line.Chunk(line.Length / 2)).Select(GetCommonItem);
        Console.WriteLine($"Part One : {markers.Sum(s => priorities[s])}");

        var badges = lines.Chunk(3).Select(GetCommonItem);
        Console.WriteLine($"Part Two : {badges.Sum(s => priorities[s])}");

        Console.ReadKey();
    }

    private static T GetCommonItem<T>(IEnumerable<IEnumerable<T>> chunk) =>
        chunk.First().First(t => chunk.All(a => a.Contains(t)));
}