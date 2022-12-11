namespace MonkeyInTheMiddle;

public abstract class Program
{
    record MonkeyBusiness(int No, List<int> Items, string[] Operation, int DivisibleBy, (int IfTrue, int IfFalse) ThrowTo, int Turns = 0)
    {
        public int Turns { get; set; } = Turns;
    }

    public static void Main()
    {
        var inputs = File.ReadAllText("inputs-demo.txt")
            .Split($"{Environment.NewLine}{Environment.NewLine}")
            .Select(s => s.Split(Environment.NewLine).ToArray()).ToArray();

        var monkeyBusiness = ParseMonkeyBusiness(inputs);

        for (var sling = 1; sling <= 20; sling++)
        {
            foreach (var monkey in monkeyBusiness)
            {
                var itemsCount = monkey.Items.Count;

                for (var i = 0; i < itemsCount; i++)
                {
                    monkey.Turns++;
                    var item = monkey.Items.First();
                    var worry = (monkey.Operation.First(), monkey.Operation.Last()) switch
                    {
                        ("*", "old") => item * item,
                        ("*", _) => item * int.Parse(monkey.Operation.Last()),
                        _ => item + int.Parse(monkey.Operation.Last()),
                    } / 3;

                    var toMonkey = worry % monkey.DivisibleBy == 0 ? monkey.ThrowTo.IfTrue : monkey.ThrowTo.IfFalse;
                    monkeyBusiness.First(f => f.No == toMonkey).Items.Add(worry);
                    monkey.Items.Remove(item);
                }
            }
        }

        Console.WriteLine($"Part One : {monkeyBusiness.OrderByDescending(o => o.Turns).Take(2).Select(s => s.Turns).Aggregate(1, (a, b) => a * b)}");
    }

    private static MonkeyBusiness[] ParseMonkeyBusiness(string[][] inputs)
    {
        return inputs.Select(input =>
            new MonkeyBusiness(int.Parse(input[0].Replace("Monkey ", string.Empty).First().ToString()),
                input[1].Replace("Starting items: ", string.Empty)
                    .Split(",", StringSplitOptions.TrimEntries).Select(int.Parse).ToList(),
                input[2].Replace("Operation: new = old ", string.Empty).Trim().Split(" "),
                int.Parse(input[3].Replace("Test: divisible by ", string.Empty).Trim()),
                (int.Parse(input[4].Replace("If true: throw to monkey ", string.Empty).Trim()),
                    int.Parse(input[5].Replace("If false: throw to monkey ", string.Empty).Trim())))).ToArray();
    }
}