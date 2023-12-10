using Extensions;
using System.Linq;

internal class Program
{
    private static void Main(string[] args)
    {
        var inputs = File.ReadAllLines("inputs.txt");
        var values = inputs.Select(x => new List<List<long>>() { x.Split(' ').Select(long.Parse).ToList() }).ToArray();

        for (int row = 0; row < values.Length; row++)
        {
            bool @continue;

            do
            {
                var differences = Enumerable.Range(0, values[row][^1].Count - 1)
                    .Select(index => values[row][^1][index + 1] - values[row][^1][index]).ToList();

                values[row].Add(differences);

                @continue = differences.Any(x => x != 0);

            } while (@continue);
        }

        for (int row = 0; row < values.Length; row++)
        {
            for (int index = values[row].Count - 2; index > 0; index--)
            {
                values[row][index - 1].Add(
                    values[row][index - 1][^1] +
                    values[row][index][^1]);
            }
        }

        var sum1 = values.Sum(s => s[0][^1]);

        //Console.WriteLine($"Part One : {}");

        //Console.WriteLine($"Part Two : {}");

        Console.Read();
    }
}

namespace Extensions
{
   
}