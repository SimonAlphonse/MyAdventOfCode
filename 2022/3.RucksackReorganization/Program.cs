public class Program
{
    public static void Main(string[] args)
    {
        var lines = File.ReadAllLines("inputs.txt");
        var priorities =
            Enumerable.Range(97, 26).Select((s, i) => KeyValuePair.Create((char)s, i + 1)).Concat(
            Enumerable.Range(65, 26).Select((s, i) => KeyValuePair.Create((char)s, i + 27))).ToDictionary(x => x.Key, y => y.Value);
        
        var rucksacks = lines.Select(x => (First: string.Concat(x.Take(x.Length / 2)), Second: string.Concat(x.Skip(x.Length / 2))))
            .Select(s => (s.First, s.Second, Common: Enumerable.Intersect(s.First, s.Second).First()))
            .Select(s => (s.First, s.Second, s.Common, Priority: priorities[s.Common])).ToList();

        Console.WriteLine($"First : {rucksacks.Sum(s => s.Priority)}");

        var groups = lines.Select(s => string.Concat(s.Distinct())).Chunk(3).ToList();
        var flattenGroups = groups.Select(s => string.Concat(s)).ToList();
        var badges = flattenGroups.Select(s => s.GroupBy(g => g).OrderByDescending(o => o.Count()).First().Key)
            .Select(s => (Badge: s, Priority: priorities[s])).ToList();

        Console.WriteLine($"Second : {badges.Sum(s => s.Priority)}");

        Console.ReadKey();
    }
}