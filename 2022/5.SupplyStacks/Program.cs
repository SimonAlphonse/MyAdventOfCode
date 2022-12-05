using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace SupplyStacks
{
    internal class Program
    {
        record Instruction(int Count, int From, int To);

        static void Main(string[] args)
        {
            var inputs = File.ReadAllText($"inputs.txt")
                .Split($"{Environment.NewLine}{Environment.NewLine}")
                .Select(s => s.Split($"{Environment.NewLine}")).ToList();

            var supplies = inputs.First().TakeWhile(x => x.Contains("[")).Reverse()
                .Select(s => s.Chunk(4).Select(s => string.Concat(s).Trim()).ToList()).ToList();

            var stacks = new Dictionary<int, List<string>>();

            for (int stackIndex = 1; stackIndex <= supplies.Select(s => s.Count).Max(); stackIndex++)
                stacks.Add(stackIndex, supplies.Select(s => s.Skip(stackIndex - 1).First()).Where(w => !string.IsNullOrWhiteSpace(w)).ToList());

            var instructions = inputs.Last().Select(s =>
                s.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                 .Where(w => int.TryParse(w, out int temp))
                 .Select(int.Parse))
                 .Select(x => new Instruction (Count: x.First(), From: x.Skip(1).First(), To: x.Last())).ToList();

            foreach (var instruction in instructions)
            {
                var sourceStack = stacks[instruction.From];
                var suppliesToMove = sourceStack.TakeLast(instruction.Count).ToArray();

                stacks[instruction.To].AddRange(suppliesToMove.Reverse());
                stacks[instruction.From].RemoveRange(stacks[instruction.From].Count - instruction.Count, instruction.Count);
            }

            Console.WriteLine($"Part One : {string.Concat(stacks.Select(s => s.Value.Last())).Replace("[", string.Empty).Replace("]", string.Empty)}");

            Console.Read();
        }
    }
}