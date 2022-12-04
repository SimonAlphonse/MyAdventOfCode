using System.Runtime.ExceptionServices;
using System.Security.AccessControl;

namespace CampCleanup
{
    public class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("inputs.txt");

            var elfsWithIds = lines.Select(a => a.Split(',')
                                   .Select(b => b.Split('-')
                                   .Select(int.Parse).FillRange().ToList()).ToList())
                                   .Select(d => (
                                        First: d.First(),
                                        Second: d.Last(),
                                        Common: d.First().Intersect(d.Last()).ToList()
                                    )).ToList();

            var vettiElfsCount = elfsWithIds.Count(s => s.Common.SequenceEqual(s.First) || s.Common.SequenceEqual(s.Second));

            Console.WriteLine($"First : {vettiElfsCount}");

            var overlappedElfsCount = elfsWithIds.Count(s => s.Common.Any());

            Console.WriteLine($"Second : {overlappedElfsCount}");

            Console.ReadKey();
        }
    }

    public static class Extensions
    {
        public static IEnumerable<int> FillRange(this IEnumerable<int> range)
        {
            var min = range.Min();
            return Enumerable.Range(min, range.Max() - min + 1);
        }
    }
}