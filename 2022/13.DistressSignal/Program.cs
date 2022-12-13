using Newtonsoft.Json.Linq;

namespace DistressSignal;

abstract class Program
{
    record Signal(string Left, string Right);
    record Pair(string Key, string Value);
    record Packet(int[] Elements, string Packets);

    public static void Main()
    {
        var inputs = File.ReadAllText("inputs.txt")
            .Split($"{Environment.NewLine}{Environment.NewLine}")
            .Select(s => new Signal(s.Split(Environment.NewLine).First(),
                s.Split(Environment.NewLine).Last())).ToArray();

        var partOne = inputs.Select((signal, index) => (Index: index + 1, 
            IsOrdered: CheckOrder(JArray.Parse(signal.Left), JArray.Parse(signal.Right), null))).ToArray();

        Console.WriteLine($"Part One : {partOne.Where(w => w.IsOrdered ?? true).Sum(s => s.Index)}");

        Console.Read();
    }

    private static bool? CheckOrder(JArray leftArray, JArray rightArray, bool? isOrdered)
    {
        for (var i = 0; i < leftArray.Count; i++)
        {
            JToken left = leftArray.Skip(i).First();
            JToken? right = rightArray.Skip(i).FirstOrDefault();

            switch (left, right)
            {
                case (JValue intLeft, JValue intRight) when intLeft.Value<int>() != intRight.Value<int>():
                    isOrdered = intLeft.Value<int>() < intRight.Value<int>();
                    break;
                case (JValue intLeft, JValue intRight) when intLeft.Value<int>() == intRight.Value<int>():
                    break;
                case (JArray arrayLeft, JArray arrayRight):
                    isOrdered = CheckOrder(arrayLeft, arrayRight, isOrdered);
                    break;
                case (JArray arrayLeft, JValue intRight):
                    isOrdered = CheckOrder(arrayLeft, new JArray() { intRight.Value<int>() }, isOrdered);
                    break;
                case (JValue intLeft, JArray arrayRight):
                    isOrdered = CheckOrder(new JArray() { intLeft.Value<int>() }, arrayRight, isOrdered);
                    break;
                case (_, null):
                    isOrdered = false;
                    break;
                default:
                    throw new InvalidDataException($"{left}-{right}");
            }

            if (isOrdered.HasValue) return isOrdered;
        }

        return isOrdered;
    }
}