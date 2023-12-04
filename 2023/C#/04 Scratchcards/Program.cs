using Extensions;

internal class Program
{
    private static void Main(string[] args)
    {
        var lines = File.ReadAllLines("inputs.txt");

        var cards = lines.Select(s => s.Split(':')).Select(s => (int.Parse(s.First()[4..]),
            s.Last().Split('|').Select(s => s.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.Parse(s)))));

        var wins = cards.Select(s => (s.Item1, s.Item2.First().Intersect(s.Item2.Last()).Count()));
        var sum1 = wins.Where(w => w.Item2 > 0).Sum(s => Math.Pow(2, s.Item2 - 1));

        Console.WriteLine($"Part One : {sum1}");

        var lookup = wins.ToDictionary(s => s.Item1, s => Enumerable.Range(s.Item1 + 1, s.Item2));
        var group = lookup.GroupBy(g => g.Value.Count() > 0).ToDictionary(s => s.Key, s => s.Select(s => s.Key));

        List<int> scratched = [.. group[false]], pile = [.. group[true]];

        do
        {
            var copies = pile.SelectMany(s => lookup[s]).ToArray();
            scratched.AddRange(pile);
            pile.Clear();
            pile.AddRange(copies);
        } while (pile.Count > 0);

        Console.WriteLine($"Part Two : {scratched.Count}");

        Console.Read();
    }
}

namespace Extensions
{
   
}