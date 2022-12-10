using System.Diagnostics;

namespace CathodeRayTube
{
    internal abstract class Program
    {
        record Instruction(string Command, int Cycle, int Value);
        record Cycle(int No, int Register, int[] Spire);

        public static void Main()
        {
            var instructions = File.ReadAllLines($"inputs-demo.txt")
                .Select(s => s.Replace("noop", "noop 1 0")
                    .Replace("addx ", "addx 2 ")).Select(s => s.Split(" "))
                .Select(s => new Instruction(s[0], int.Parse(s[1]), int.Parse(s[2]))).ToList();

            List<int> registers = new() { 1 };
            foreach (var instruction in instructions)
                for (var i = 1; i <= instruction.Cycle; i++)
                    registers.Add(registers.Last() + (i == instruction.Cycle ? instruction.Value : 0));

            var bookmarks = new[] { 20, 60, 100, 140, 180, 220 };
            var result = registers.Where((value, index) => bookmarks.Contains(index + 1))
                .Select((value, index) => value * bookmarks[index]).Sum();

            Console.WriteLine($"Part One : {result}");

            char[] crt = new string(' ', 40 * 6).ToCharArray();

            var cycles = registers.Select((s, i) => new Cycle(i, s, new[] { s - 1, s, s + 1 })).ToArray();

            foreach (var cycle in cycles.Skip(1))
            {
                if (cycle.Spire.Contains(cycle.No - 1) || cycles[cycle.No - 1].Spire.Contains(cycle.No - 1))
                    crt[cycle.No - 1] = '#';
                else
                    crt[cycle.No - 1] = '.';
            }

            PrettyPrint(crt);
        }

        private static void PrettyPrint(char[] crtLine)
        {
            foreach (var line in crtLine.Chunk(40).Select(s => string.Concat(s)))
            {
                Debug.WriteLine(line);
                Console.WriteLine(line);
            }
        }
    }
}