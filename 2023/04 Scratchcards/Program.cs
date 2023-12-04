using Extensions;
using System.Drawing;

internal class Program
{
    private static void Main(string[] args)
    {
        var lines = File.ReadAllLines("inputs.txt");

        var cards = lines.Select(s => s.Split(':')).Select(s => (int.Parse(s.First()[4..]),
            s.Last().Split('|').Select(s => s.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.Parse(s)).ToArray()).ToArray())).ToArray();

        var sum1 = cards.Select(s => (s.Item1, s.Item2.First().Intersect(s.Item2.Last()).Count()))
            .Where(w => w.Item2 > 0).Sum(s => Math.Pow(2, s.Item2 - 1));

        Console.WriteLine($"Part One : {sum1}");

        //Console.WriteLine($"Part Two : {}");

        Console.Read();
    }
}

namespace Extensions
{
   
}