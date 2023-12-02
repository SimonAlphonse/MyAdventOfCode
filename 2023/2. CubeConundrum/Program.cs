var lines = File.ReadLines("inputs.txt");

var criteria = new Rgb() { Red = 12, Green = 13, Blue = 14 };

var games = lines.Select(line => new Game
(
    int.Parse(line.Split(':').First()[4..]),
    ParseSets(line.Split(": ").Last())
)).ToList();

var sum1 = games.Where(w => w.Rgb.All(a => 
        a.Red <= criteria.Red && 
        a.Green <= criteria.Green && 
        a.Blue <= criteria.Blue))
    .Select(s=>s.SNo).Sum();

Console.WriteLine($"Part One : {sum1}");

//Console.WriteLine($"Part Two : {sum2}");

Console.Read();

static Rgb[] ParseSets(string sets)
{
    return sets.Split("; ").Select(set =>
            ParseRgb(set)).ToArray();
}

static Rgb ParseRgb(string set)
{
    Rgb rgb = new();

    foreach (var item in set.Split(", "))
    {
        if (item.EndsWith(Color.red.ToString()))
            rgb.Red = int.Parse(item[..^3]);
        else if (item.EndsWith(Color.green.ToString()))
            rgb.Green = int.Parse(item[..^5]);
        else if (item.EndsWith(Color.blue.ToString()))
            rgb.Blue = int.Parse(item[..^4]);
    }

    return rgb;
}

record Game(int SNo, Rgb[] Rgb);

public record Rgb
{
    public int Red { get; set; }
    public int Green { get; set; }
    public int Blue { get; set; }
}

enum Color
{
    red = 0,
    blue = 1,
    green = 2,
}