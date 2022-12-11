namespace MonkeyInTheMiddle;

public abstract class Program
{
    record Choice(int IfTrue, int IfFalse);
    record MonkeyBusiness(int No, Queue<ulong> Items, string[] Operation, ulong Divisor, Choice ThrowTo)
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
        return WatchMonkeyBusiness(rounds, monkeys, relax)
            .Select(s => s.Inspection)
            .OrderByDescending(o => o)
            .Take(2).Aggregate((a, b) => a * b);
    }

    private static MonkeyBusiness[] WatchMonkeyBusiness(int rounds, MonkeyBusiness[] monkeys, Func<ulong, ulong> relax)
    {
        for (var round = 1; round <= rounds; round++)
        {
            foreach (var monkey in monkeys)
            {
                while (monkey.Items.TryDequeue(out var item))
                {
                    monkey.Inspection++;
                    var worry = relax(WatchAboutInspection(monkey, item));
                    var toMonkey = worry % monkey.Divisor == 0 ? monkey.ThrowTo.IfTrue : monkey.ThrowTo.IfFalse;
                    monkeys.First(f => f.No == toMonkey).Items.Enqueue(worry);
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
                    new(input[1].Replace("Starting items: ", string.Empty)
                        .Split(",", StringSplitOptions.TrimEntries).Select(ulong.Parse)),
                    input[2].Replace("Operation: new = old ", string.Empty).Trim().Split(" "),
                    ulong.Parse(input[3].Replace("Test: divisible by ", string.Empty).Trim()),
                    new(int.Parse(input[4].Replace("If true: throw to monkey ", string.Empty).Trim()),
                        int.Parse(input[5].Replace("If false: throw to monkey ", string.Empty).Trim())))).ToArray();
    }
}