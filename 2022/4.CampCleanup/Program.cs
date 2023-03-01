namespace CampCleanup
{
    public abstract class Program
    {
        private static void Main()
        {
            var lines = File.ReadAllLines("inputs.txt");

            var elvesWithIds = lines.Select(a => a.Split(',')
                                   .Select(b => b.Split('-')
                                   .Select(int.Parse).ToArray().FillRange()).ToArray())
                                   .Select(d => (
                                        First: d.First(),
                                        Second: d.Last(),
                                        Common: d.First().Intersect(d.Last()).ToList()
                                    )).ToList();

            Console.WriteLine($"Part One : {elvesWithIds.Count(s => s.Common.SequenceEqual(s.First) || s.Common.SequenceEqual(s.Second))}");
            Console.WriteLine($"Part Two : {elvesWithIds.Count(s => s.Common.Any())}");

            Console.ReadKey();
        }
    }

    public static class Extensions
    {
        public static int[] FillRange(this int[] range)
        {
            var min = range.Min();
            return Enumerable.Range(min, range.Max() - min + 1).ToArray();
        }
    }
}