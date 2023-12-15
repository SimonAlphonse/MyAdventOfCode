using Extensions;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

internal class Program
{
    private static void Main(string[] args)
    {
        char[] numbers = ['2', '3', '4', '5', '6', '7', '8', '9', 'T'];
        char[] special = ['J', 'Q', 'K', 'A'];
        char[] labels = [.. numbers, .. special];

        Func<char, bool> isNumber = x => x <= '9' || x == 'T';
        Func<char, bool> isSpecial = x => x > '9' && x != 'T';

        var weights = labels.ToIndexedDictionary();

        var bids = File.ReadAllLines("sample.txt").ToDictionary(s => s.Split(' ').First(), s => int.Parse(s.Split(' ').Last()));

        var k = bids.Select(hand => hand.Key.Select(label => (label, hand.Key.Count(c => c == label))).Distinct())
            .GroupBy(g => (pair: g.Max(m => m.Item2), length: g.Count(),
                isAllNumber: g.Select(s => s.label).All(isNumber), isAllSpecial: g.Select(s => s.label).All(isSpecial))).ToArray();

        //Console.WriteLine($"Part One : {}");

        //Console.WriteLine($"Part Two : {}");

        Console.Read();
    }

    static PairType GetPairType(int max, int length, bool isSame)
    {
        return (max, length, isSame) switch
        {
            (_, _, true) => PairType.FullHouse,
            (3, _, _) => PairType.ThreeOfAKind,
            (2, _, _) => PairType.TwoPair,
            _ => PairType.SinglePair
        };
    }

    enum PairType
    {
        SinglePair,
        TwoPair,
        ThreeOfAKind,
        FullHouse,
    }
}

namespace Extensions
{
    public static class GenericExtensions
    {
        public static Dictionary<int, T> ToIndexedDictionary<T>(this IEnumerable<T> array)
        {
            return array.Select((value, index) => (value, index))
                .ToDictionary(x => x.index, x => x.value);
        }
    }
}