using Extensions;

internal class Program
{
    private static void Main(string[] args)
    {
        var inputs = File.ReadAllLines("inputs.txt")
            .Select(s => s.Split(':').Last()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse).ToArray()).ToArray();

        var records = Enumerable.Range(0, inputs.Length + 1)
            .Select(i => (Time: inputs[0][i], Distance: inputs[1][i])).ToArray();

        int[] newRecords = GetRecordCounts(records).ToArray();

        Console.WriteLine($"Part One : {newRecords.Aggregate((a, b) => a * b)}");

        //Console.WriteLine($"Part Two : {}");

        Console.Read();
    }

    private static IEnumerable<int> GetRecordCounts((int Time, int Distance)[] records)
    {
        foreach (var record in records)
        {
            yield return Enumerable.Range(1, record.Time - 1)
                .Select(hold => hold * (record.Time - hold))
                .Where(w => w > record.Distance).Count();
        }
    }
}

namespace Extensions
{
   
}