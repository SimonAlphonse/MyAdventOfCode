var lines = File.ReadAllText("inputs.txt");
var elves = lines.Split($"{Environment.NewLine}{Environment.NewLine}")
    .Select(s=> s.Split($"{Environment.NewLine}").Select(int.Parse).ToList()).ToList();

var elvesByCalories = elves.OrderByDescending(o => o.Sum()).ToList();

Console.WriteLine($"Part One : {elvesByCalories.First().Sum()}");

Console.WriteLine($"Part Two : {elvesByCalories.Take(3).SelectMany(s => s).Sum()}");

Console.Read();