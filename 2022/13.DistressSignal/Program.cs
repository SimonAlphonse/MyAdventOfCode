using Newtonsoft.Json.Linq;

namespace DistressSignal;

internal abstract class Program
{
    private record Signal(string Left, string Right);

    public static void Main()
    {
        var inputOne = File.ReadAllText("inputs.txt")
            .Split($"{Environment.NewLine}{Environment.NewLine}")
            .Select(s => new Signal(s.Split(Environment.NewLine).First(),
                s.Split(Environment.NewLine).Last())).ToArray();

        var partOne = inputOne.Select((signal, index) => (Index: index + 1, Signal: signal,
            IsOrdered: CheckOrder(JArray.Parse(signal.Left), JArray.Parse(signal.Right)))).ToArray();
        Console.WriteLine($"Part One : {partOne.Where(w => w.IsOrdered ?? true).Sum(s => s.Index)}");

        var inputTwo = File.ReadAllLines("inputs.txt").Where(w => !string.IsNullOrEmpty(w)).ToList();
        inputTwo.AddRange(new[] { "[[2]]", "[[6]]" });

        var arrangedPackets = SortPackets(inputTwo);
        Console.WriteLine($"Part Two : {(arrangedPackets.IndexOf("[[2]]") + 1)
                                        * (arrangedPackets.IndexOf("[[6]]") + 1)}");

        Console.Read();
    }

    private static bool? CheckOrder(JArray leftArray, JArray rightArray)
    {
        bool? isOrdered = null;

        for (var i = 0; i < int.Max(leftArray.Count, rightArray.Count); i++)
        {
            JToken? left = leftArray.Skip(i).FirstOrDefault();
            JToken? right = rightArray.Skip(i).FirstOrDefault();

            switch (left, right)
            {
                case (JValue intLeft, JValue intRight) when intLeft.Value<int>() != intRight.Value<int>():
                    isOrdered = intLeft.Value<int>() < intRight.Value<int>();
                    break;
                case (JValue intLeft, JValue intRight) when intLeft.Value<int>() == intRight.Value<int>():
                    break;
                case (JArray arrayLeft, JArray arrayRight) when arrayLeft.Any() && !arrayRight.Any():
                    isOrdered = false;
                    break;
                case (JArray arrayLeft, JArray arrayRight) when !arrayLeft.Any() && arrayRight.Any():
                    isOrdered = true;
                    break;
                case (JArray arrayLeft, JArray arrayRight):
                    isOrdered = CheckOrder(arrayLeft, arrayRight);
                    break;
                case (JArray arrayLeft, JValue) when !arrayLeft.Any():
                    isOrdered = true;
                    break;
                case (JArray arrayLeft, JValue intRight):
                    isOrdered = CheckOrder(arrayLeft, new JArray() { intRight.Value<int>() });
                    break;
                case (JValue, JArray arrayRight) when !arrayRight.Any():
                    isOrdered = false;
                    break;
                case (JValue intLeft, JArray arrayRight):
                    isOrdered = CheckOrder(new JArray() { intLeft.Value<int>() }, arrayRight);
                    break;
                case (null, _):
                    isOrdered = true;
                    break;
                case (_, null):
                    isOrdered = false;
                    break;
                default:
                    throw new InvalidDataException($"{left},{right}");
            }

            if (isOrdered.HasValue) return isOrdered;
        }

        return isOrdered;
    }

    private static IList<string> SortPackets(IList<string> packets)
    {
        for (var i = 0; i < packets.Count - 1;)
        {
            if (!CheckOrder(JArray.Parse(packets[i]), JArray.Parse(packets[i + 1])) ?? true)
            {
                packets.Swap(i, i + 1);
                i -= i == 0 ? 0 : 1;
            }
            else i++;
        }

        return packets;
    }
}

public static class ListExtensions
{
    public static void Swap<T>(this IList<T> list, int from, int to) => (list[from], list[to]) = (list[to], list[from]);
}