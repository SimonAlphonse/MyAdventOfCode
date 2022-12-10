namespace CathodeRayTube
{
    internal abstract class Program
    {
        record Instruction(string Command, int Cycle, int Value);

        public static void Main()
        {
            var instructions = File.ReadAllLines($"inputs.txt")
                .Select(s => s.Replace("noop", "noop 1 0")
                    .Replace("addx ", "addx 2 ")).Select(s => s.Split(" "))
                .Select(s => new Instruction(s[0], int.Parse(s[1]), int.Parse(s[2]))).ToList();

            List<int> register = new() { 1 };
            foreach (var instruction in instructions)
            {
                for (int i = 1; i <= instruction.Cycle; i++)
                {
                    register.Add(register.Last() + (i == instruction.Cycle ? instruction.Value : 0));
                }
            }

            var bookmarks = new[] { 20, 60, 100, 140, 180, 220 };

            var result = register.Where((value, index) => bookmarks.Contains(index + 1))
                .Select((value, index) => value * bookmarks[index]).Sum();

            Console.WriteLine($"Part One : {result}");
            
        }
    }
}