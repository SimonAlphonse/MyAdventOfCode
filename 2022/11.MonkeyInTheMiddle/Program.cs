namespace MonkeyInTheMiddle;

public abstract class Program
{
    record MonkeyBusiness(int No, List<ulong> Items, string[] Operation, ulong Divisor, (int IfTrue, int IfFalse) ThrowTo)
    {
        public ulong Inspection { get; set; }
    }

    public static void Main()
    {
        Console.WriteLine($"Part One Demo : {GetMonkeyBusinessLevel("inputs-demo.txt", 20, true)}");
        Console.WriteLine($"Part One : {GetMonkeyBusinessLevel("inputs.txt", 20, true)}");
        Console.WriteLine($"Part Two Demo : {GetMonkeyBusinessLevel("inputs-demo.txt", 10000, false)}");
        Console.WriteLine($"Part Two : {GetMonkeyBusinessLevel("inputs.txt", 10000, false)}");

        Console.ReadKey();
    }

    private static ulong GetMonkeyBusinessLevel(string fileName, int rounds, bool divide)
    {
        var monkeys = GetMonkeys(fileName);
        var relax = new Func<ulong, ulong>(worry => divide ? worry / 3 
            : worry % monkeys.Select(s => s.Divisor).Aggregate((x, y) => x * y));
        return WatchMonkeyBusiness(rounds, monkeys, relax).Select(s => s.Inspection)
            .OrderByDescending(o => o)
            .Take(2).Aggregate((a, b) => a * b);
    }

    private static MonkeyBusiness[] WatchMonkeyBusiness(int rounds, MonkeyBusiness[] monkeys, Func<ulong, ulong> relax)
    {
        for (var round = 1; round <= rounds; round++)
        {
            foreach (var monkey in monkeys)
            {
                var itemsCount = monkey.Items.Count;

                for (var i = 0; i < itemsCount; i++)
                {
                    monkey.Inspection++;

                    var item = monkey.Items.First();
                    var worry = WatchAboutInspection(monkey, item);
                    worry = relax(worry);

                    var toMonkey = worry % monkey.Divisor == 0 ? monkey.ThrowTo.IfTrue : monkey.ThrowTo.IfFalse;
                    monkeys.First(f => f.No == toMonkey).Items.Add(worry);
                    monkey.Items.Remove(item);

                    // Console.WriteLine($"{round} | {monkey.No}.{i} -> {toMonkey} | {item} -> {worry}");
                }
            }
        }
        
        return monkeys;
    }

    private static ulong WatchAboutInspection(MonkeyBusiness monkey, ulong item)
    {
        return (monkey.Operation.First(), monkey.Operation.Last()) switch
        {
            ("*", "old") => item * item,
            ("*", _) => item * ulong.Parse(monkey.Operation.Last()),
            _ => item + ulong.Parse(monkey.Operation.Last()),
        };
    }

    private static MonkeyBusiness[] GetMonkeys(string fileName)
    {
        return File.ReadAllText(fileName)
            .Split($"{Environment.NewLine}{Environment.NewLine}")
            .Select(s => s.Split(Environment.NewLine).ToArray()).Select(input =>
            new MonkeyBusiness(int.Parse(input[0].Replace("Monkey ", string.Empty).First().ToString()),
                input[1].Replace("Starting items: ", string.Empty)
                    .Split(",", StringSplitOptions.TrimEntries).Select(ulong.Parse).ToList(),
                input[2].Replace("Operation: new = old ", string.Empty).Trim().Split(" "),
                ulong.Parse(input[3].Replace("Test: divisible by ", string.Empty).Trim()),
                (int.Parse(input[4].Replace("If true: throw to monkey ", string.Empty).Trim()),
                    int.Parse(input[5].Replace("If false: throw to monkey ", string.Empty).Trim())))).ToArray();
    }
}