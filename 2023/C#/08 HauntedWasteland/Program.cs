using Extensions;
using System.Numerics;
using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        ParseInputs(File.ReadAllLines("inputs.txt"), out var directions, out var maps);

        long count = CalculateSteps(directions, maps);

        Console.WriteLine($"Part One : {count}");

        ParseInputs(File.ReadAllLines("inputs.txt"), out directions, out maps);

        long[] cycles = CalculateGhostSteps(directions, maps);

        Console.WriteLine($"Part Two : {cycles.Lcm()}");

        Console.Read();
    }

    private static long CalculateSteps(int[] directions, Dictionary<string, string[]> maps)
    {
        long steps = 0;
        var position = "AAA";

        do
        {
            foreach (var direction in directions)
            {
                position = maps[position][direction];
                steps++;

                if (position == "ZZZ") { break; }
            }
        } while (position != "ZZZ");
        return steps;
    }

    private static long[] CalculateGhostSteps(int[] directions, Dictionary<string, string[]> maps)
    {
        long steps = 0;

        var positions = maps.Keys.Where(s => s.EndsWith('A')).ToArray();
        long[] cycles = new long[positions.Length];

        do
        {
            foreach (var direction in directions)
            {
                Debug.WriteLine($"{string.Join(",", positions)}");

                positions = positions.Select(key => maps[key][direction]).ToArray();
                steps++;

                if (positions.Any(x => x.EndsWith('Z')))
                {
                    int[] tracks = positions.Select((v, i) => (v.EndsWith('Z'), i))
                        .Where(w => w.Item1).Select(s => s.i).ToArray();

                    foreach (var track in tracks)
                    {
                        if (cycles[track] == 0)
                            cycles[track] = steps;
                    }
                }

                if (cycles.All(a => a != 0)) // || !positions.Any(x => !x.EndsWith('Z'))
                    break;
            }
        } while (cycles.Any(a => a == 0)); // || positions.Any(x => !x.EndsWith('Z'))

        return cycles;
    }

    private static void ParseInputs(string[] lines, out int[] directions, out Dictionary<string, string[]> maps)
    {
        directions = lines[0].Select(s => s == 'L' ? 0 : 1).ToArray();
        maps = lines[2..].Select(s => s.Split('='))
                    .ToDictionary(x => x[0][..3], x => new[] { x[1][2..5], x[1][7..^1] });
    }

    record struct Map(string Left, string Right);
}

namespace Extensions
{
    public static class NumberExtension
    {
        public static T Lcm<T>(this IEnumerable<T> values) where T : INumber<T>
        {
            return values.Aggregate((a, b) => Lcm<T>(a, b));
        }

        static T Lcm<T>(T a, T b) where T : INumber<T>
        {
            return (a / Gcd(a, b)) * b;
        }

        static T Gcd<T>(T a, T b) where T : INumber<T>
        {
            while (b != T.Zero)
            {
                T temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }
    }

    //TODO - write indexer for 2d array with single []
}