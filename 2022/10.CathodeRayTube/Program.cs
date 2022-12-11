using System.Diagnostics;

namespace CathodeRayTube
{
    internal abstract class Program
    {
        record Instruction(string Command, int Cycle, int Value);
        record Cycle(int No, int[] Spire);
        record State(Instruction Instruction, int From, int To);

        public static void Main()
        {
            var instructions = File.ReadAllLines($"inputs.txt")
                .Select(s => s.Replace("noop", "noop 1 0")
                    .Replace("addx ", "addx 2 ")).Select(s => s.Split(" "))
                .Select(s => new Instruction(s[0], int.Parse(s[1]), int.Parse(s[2]))).ToList();

            var registers = ExecuteInstructions(instructions);
            
            Console.WriteLine($"Part One : {GetSignalStrength(registers)}");

            char[] crt = new string(' ', 40 * 6).ToCharArray();

            var cycles = registers.Skip(1).Select((s, i) =>
                new Cycle(i + 1, new[] { s.From % 40 - 1, s.From % 40, s.From % 40 + 1 })).ToArray();

            foreach (var cycle in cycles)
                crt[cycle.No - 1] = cycle.Spire.Contains(Math.Max(cycle.No % 40 - 1, 0)) ? '#' : '.';

            PrettyPrint(crt);
        }

        private static int GetSignalStrength(List<State> registers)
        {
            var bookmarks = Enumerable.Range(-20, 6).Select(s => s + 40).ToArray();
            return registers.Where((_, index) => bookmarks.Contains(index + 1))
                .Select((value, index) => value.From * bookmarks[index]).Sum();
        }

        private static List<State> ExecuteInstructions(List<Instruction> instructions)
        {
            List<State> registers = new() { new State(new("Start", 0, 0), 1, 1) };

            foreach (var instruction in instructions)
                for (var i = 1; i <= instruction.Cycle; i++)
                    registers.Add(i == instruction.Cycle
                        ? new State(instruction, registers.Last().To, registers.Last().To + instruction.Value)
                        : new State(instruction, registers.Last().To, registers.Last().To));

            return registers;
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