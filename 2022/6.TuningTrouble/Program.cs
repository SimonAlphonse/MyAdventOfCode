namespace TuningTrouble
{
    internal class Program
    {
        static void Main()
        {
            Console.WriteLine($"Part One :{FindMarkerIndex(4)}"); ;
            Console.WriteLine($"Part Two :{FindMarkerIndex(14)}"); ;

            Console.Read();
        }

        private static int FindMarkerIndex(int markerLength)
        {
            var lines = File.ReadAllText($"inputs.txt");

            for (int i = 0; i < lines.Length - markerLength - 1; i++)
            {
                string marker = string.Concat(lines.Skip(i).Take(markerLength));
                if (marker.Distinct().Count() == markerLength)
                {
                    return i + markerLength;
                }
            }

            return 0;
        }
    }
}