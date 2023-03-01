using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace SupplyStacks
{
    internal class Program
    {
        record Instruction(int Count, int From, int To);

        static void Main()
        {
            var inputs = File.ReadAllText($"inputs.txt")
                .Split($"{Environment.NewLine}{Environment.NewLine}")
                .Select(s => s.Split($"{Environment.NewLine}")).ToList();

            var supplies = inputs.First().TakeWhile(x => x.Contains("[")).Reverse()
                .Select(x => x.Chunk(4).Select(y => string.Concat(y).Trim()).ToList()).ToList();

            var instructions = inputs.Last().Select(s =>
                s.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                 .Where(w => int.TryParse(w, out _))
                 .Select(int.Parse).ToList())
                 .Select(x => new Instruction(Count: x.First(), From: x.Skip(1).First(), To: x.Last())).ToList();

            Console.WriteLine($"Part One : {RearrangeSupplies(supplies, instructions, true)}");
            Console.WriteLine($"Part Two : {RearrangeSupplies(supplies, instructions, false)}");

            Console.Read();
        }

        private static string RearrangeSupplies(List<List<string>> supplies, List<Instruction> instructions, bool isSinglePickup)
        {
            var stacks = new Dictionary<int, List<string>>();

            for (int stackIndex = 1; stackIndex <= supplies.Select(s => s.Count).Max(); stackIndex++)
                stacks.Add(stackIndex, supplies.Select(s => s.Skip(stackIndex - 1).First()).Where(w => !string.IsNullOrWhiteSpace(w)).ToList());

            foreach (var instruction in instructions)
            {
                var sourceStack = stacks[instruction.From];
                var suppliesToMove = sourceStack.TakeLast(instruction.Count).ToArray();

                stacks[instruction.To].AddRange(isSinglePickup ? suppliesToMove.Reverse() : suppliesToMove);
                stacks[instruction.From].RemoveRange(stacks[instruction.From].Count - instruction.Count, instruction.Count);
            }

            return string.Concat(stacks.Select(s => s.Value.Last()))
                .Replace("[", string.Empty)
                .Replace("]", string.Empty);
        }
    }
}