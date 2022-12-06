namespace TuningTrouble
{
    internal class Program
    {
        static void Main()
        {
            var inputs = File.ReadAllText($"inputs.txt");
            Console.WriteLine($"Part One : {FindMarkerIndex(inputs, 4)}");
            Console.WriteLine($"Part Two : {FindMarkerIndex(inputs, 14)}");
            Console.Read();
        }

        private static int FindMarkerIndex(string inputs, int markerLength)
        {
            for (int i = 0; i < inputs.Length - markerLength - 1; i++)
            {
                if (inputs.Skip(i).Take(markerLength).Distinct().Count() == markerLength)
                    return i + markerLength;
            }

            return 0;
        }
    }
}