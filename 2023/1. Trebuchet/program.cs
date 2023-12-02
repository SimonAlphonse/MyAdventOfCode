using System.Linq;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System;

public class Trebuchet
{
    public static void Main()
    {

        var lines = File.ReadLines("inputs.txt");

        int sum1 = GetCalibrationValue(lines);

        Console.WriteLine($"Part One : {sum1}");

        Dictionary<string, char> lookup = new()
        {
            { "one", '1' },
            { "two", '2' },
            { "three", '3' },
            { "four", '4' },
            { "five", '5' },
            { "six", '6' },
            { "seven", '7' },
            { "eight", '8' },
            { "nine", '9' }
        };

        int min = 3, max = 5;

        List<char> lefts = [], rights = [];

        for (int lineNo = 0; lineNo < lines.Count(); lineNo++)
        {
            var line = lines.Skip(lineNo).First();

            if (line.Length < 3)
            {
                char left = line.First(lookup.ContainsValue); lefts.Add(left);
                char right = line.Last(lookup.ContainsValue); rights.Add(right);
                continue;
            }

            int leftIndex = 0;
            GetLeftCalibrationValues(lookup, min, max, lefts, line, ref leftIndex);

            int rightIndex = line.Length - min;
            GetRightCalibrationValues(lookup, min, max, rights, line, ref rightIndex);
        }

        var sum2 = Enumerable.Range(0, lines.Count()).Select(i => int.Parse($"{lefts[i]}{rights[i]}")).Sum();

        Console.WriteLine($"Part Two : {sum2}");

        Console.Read();
    }


    private static void GetLeftCalibrationValues(Dictionary<string, char> lookup, int min, int max, List<char> lefts, string line, ref int index)
    {
        var @length = min;
        do
        {
            var scan = line[index..(index + @length)];
            char left = scan.FirstOrDefault(lookup.ContainsValue);

            if (left > 0)
            {
                lefts.Add(left); return;
            }
            else if (lookup.Keys.Any(a => a.StartsWith(scan)) && @length <= max)
            {
                if (lookup.TryGetValue(scan, out char value))
                {
                    lefts.Add(value); return;
                }
                else
                {
                    if (@length < max)
                        @length++;
                }
            }
            else { index++; @length = min; }

        } while (index + min <= line.Length);
    }

    private static void GetRightCalibrationValues(Dictionary<string, char> lookup, int min, int max, List<char> rights, string line, ref int index)
    {
        var @length = min;
        do
        {
            var scan = line[index..(index + @length)];
            char right = scan.LastOrDefault(lookup.ContainsValue);

            if (right > 0)
            {
                rights.Add(right); return;
            }
            else if (lookup.Keys.Any(a => a.StartsWith(scan)))
            {
                if (lookup.TryGetValue(scan, out char value))
                {
                    rights.Add(value); return;
                }
                else
                {
                    if (@length < max)
                        @length++;
                }
            }
            else { index--; @length = min; }

        } while (index + min <= line.Length);
    }

    private static int GetCalibrationValue(IEnumerable<string> lines)
    {
        return lines.Select(line => line)
            .Select(s => s.Where(w => int.TryParse(w.ToString(), out _))
            .Select(x => int.Parse(x.ToString())))
            .Select(s => $"{s.First()}{s.Last()}")
            .Select(s => int.Parse(s)).Sum();
    }
}