using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace DistressSignal;

abstract class Program
{
    record Signal(string Left, string Right);

    public static void Main()
    {
        var inputs = File.ReadAllText("inputs.txt")
            .Split($"{Environment.NewLine}{Environment.NewLine}")
            .Select(s => new Signal(s.Split(Environment.NewLine).First(),
                s.Split(Environment.NewLine).Last())).ToArray();

        List<(int Index, Signal Signal, bool? IsOrdered)> partOne = new();

        for (int i = 0; i < inputs.Length; i++)
        {
            var isOrdered = CheckOrder(JArray.Parse(inputs[i].Left), JArray.Parse(inputs[i].Right));
            Console.WriteLine($"[{i+1}] [{isOrdered}]{Environment.NewLine}{Environment.NewLine
            }== LEFT : {inputs[i].Left} =={Environment.NewLine}== RIGHT : {inputs[i].Right} =={Environment.NewLine}");
            partOne.Add(new(i + 1, inputs[i], isOrdered));
        }
        
        // var partOne = inputs.Select((signal, index) => (Index: index + 1, Signal: signal,
        //     IsOrdered: CheckOrder(JArray.Parse(signal.Left), JArray.Parse(signal.Right)))).ToArray();

        Console.WriteLine($"Part One : {partOne.Where(w => w.IsOrdered ?? true).Sum(s => s.Index)}");

        Console.Read();
    }

    private static bool? CheckOrder(JArray leftArray, JArray rightArray)
    {
        bool? isOrdered = null;
        
        for (var i = 0; i < leftArray.Count; i++)
        {
            JToken left = leftArray.Skip(i).First();
            JToken? right = rightArray.Skip(i).FirstOrDefault();

            switch (left, right)
            {
                case (JValue intLeft, JValue intRight) when intLeft.Value<int>() != intRight.Value<int>():
                    isOrdered = intLeft.Value<int>() < intRight.Value<int>();
                    Console.WriteLine(isOrdered.Value
                        ? "Left side is smaller, so inputs are in the right order"
                        : "Right side is smaller, so inputs are not in the right order");
                    Console.WriteLine();
                    break;
                case (JValue intLeft, JValue intRight) when intLeft.Value<int>() == intRight.Value<int>():
                    break;
                case (JArray arrayLeft, JArray arrayRight):
                    isOrdered = CheckOrder(arrayLeft, arrayRight);
                    break;
                case (JArray arrayLeft, JValue intRight):
                    isOrdered = CheckOrder(arrayLeft, new JArray() { intRight.Value<int>() });
                    break;
                case (JValue intLeft, JArray arrayRight):
                    isOrdered = CheckOrder(new JArray() { intLeft.Value<int>() }, arrayRight);
                    break;
                case (_, null):
                    Console.WriteLine("Right side ran out of items, so inputs are not in the right order");
                    Console.WriteLine();
                    isOrdered = false;
                    break;
                default:
                    throw new InvalidDataException($"{left},{right}");
            }

            if (isOrdered.HasValue) return isOrdered;
        }

        return isOrdered;
    }
}