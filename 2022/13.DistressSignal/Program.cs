namespace DistressSignal;

abstract class Program
{
    record Signal(string Left, string Right);
    public static void Main()
    {
        var inputs = File.ReadAllText("inputs-demo.txt")
            .Split($"{Environment.NewLine}{Environment.NewLine}")
            .Select(s => new Signal(
                s.Split(Environment.NewLine).First(), 
                s.Split(Environment.NewLine).Last())
            ).ToArray().ToArray();

        
        
        
        Console.Read();
    }
}