namespace ProboscideaVolcanium;

internal abstract class Program
{
    record Valve(string Id, int FlowRate, string[] Valves);

    public static void Main()
    {
        var inputs = File.ReadAllLines("inputs-demo.txt")
            .Select(s => s.Replace(";", string.Empty).Replace(",", string.Empty)
                .Split(' ')).Select(s => new Valve(s[1], int.Parse(s[4][5..]), s[9..])).ToArray();

    }
}