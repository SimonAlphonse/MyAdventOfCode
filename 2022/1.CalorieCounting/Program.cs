
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

var lines = File.ReadAllLines("inputs.txt");

List<List<int>> elves = new() { new () };

foreach (var line in lines)
{
    List<int> calories = elves.Last();

    if (int.TryParse(line, out var calorie))
        calories.Add(calorie);
    else
        elves.Add(new ());
}

var elvesByCalories = elves.OrderByDescending(o => o.Sum()).ToList();

var maxCalories = elvesByCalories.First().Sum();

Console.WriteLine($"First : {maxCalories}");

var top3Calories = elvesByCalories.Take(3).SelectMany(s => s).Sum();

Console.WriteLine($"Second : {top3Calories}");

Console.Read();