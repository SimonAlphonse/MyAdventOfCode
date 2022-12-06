namespace TuningTrouble
{
    internal class Program
    {
        static void Main()
        {
            var inputs = File.ReadAllText($"inputs.txt");
            Console.WriteLine($"Part One : {FindMarker(inputs, 4)}");
            Console.WriteLine($"Part Two : {FindMarker(inputs, 14)}");
            Console.Read();
        }

        private static int FindMarker(string inputs, int length)
        {
            return Enumerable.Range(0, inputs.Length - length)
                .First(i => inputs.Skip(i).Take(length).Distinct().Count() == length) + length;
        }
    }
}