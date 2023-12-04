var lines = File.ReadLines("inputs.txt");

var criteria = new Rgb() { Red = 12, Green = 13, Blue = 14 };

var games = lines.Select(line => new Game
(
    int.Parse(line.Split(':').First()[4..]),
    line.Split(": ").Last().Split("; ").Select(ParseRgb).ToArray()
)).ToArray();

var sum1 = games.Where(w => w.Rgb.All(a => 
        a.Red <= criteria.Red && 
        a.Green <= criteria.Green && 
        a.Blue <= criteria.Blue))
    .Select(s=>s.SNo).Sum();

Console.WriteLine($"Part One : {sum1}");

var sum2 = games.Sum(s => 
    s.Rgb.Max(m => m.Red) * 
    s.Rgb.Max(m => m.Green) * 
    s.Rgb.Max(m => m.Blue));

Console.WriteLine($"Part Two : {sum2}");

Console.Read();

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
record Rgb
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