namespace RucksackReorganization;

public abstract class Program
{
    public static void Main()
    {
        var lines = File.ReadAllLines("inputs.txt");

        var priorities =
            Enumerable.Range('a', 'z' - 'a' + 1).Select((s, i) => KeyValuePair.Create((char)s, i + 1)).Concat(
            Enumerable.Range('A', 'Z' - 'A' + 1).Select((s, i) => KeyValuePair.Create((char)s, i + 1 + 26)))
                .ToDictionary(x => x.Key, y => y.Value);

        var rucksacks = (from line in lines
                let mid = line.Length / 2
                select (First: line.Take(mid), Second: line.TakeLast(mid)))
            .Select(s => (s.First, s.Second, Common: s.First.Intersect(s.Second).First()))
            .Select(s => (s.First, s.Second, s.Common, Priority: priorities[s.Common])).ToList();

        Console.WriteLine($"Part One : {rucksacks.Sum(s => s.Priority)}");

        var groups = lines.Select(s => string.Concat(s.Distinct())).Chunk(3).ToList();
        var flattenGroups = groups.Select(s => string.Concat(s)).ToList();
        var badges = flattenGroups.Select(s => s.GroupBy(g => g)
                .OrderByDescending(o => o.Count()).First().Key)
                .Select(s => (Badge: s, Priority: priorities[s])).ToList();

        Console.WriteLine($"Part Two : {badges.Sum(s => s.Priority)}");

        Console.ReadKey();
    }
}