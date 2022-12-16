namespace ProboscideaVolcanium;

internal abstract class Program
{
    record Valve(string Id, int FlowRate, string[] Valves);

    public static void Main()
    {
        var valves = File.ReadAllLines("inputs-demo.txt")
            .Select(s => s.Replace(",", string.Empty).Split(' '))
            .Select(s => new KeyValuePair<string, Valve>(s[1], new Valve(s[1], int.Parse(s[4][5..^1]), s[9..])))
            .ToDictionary(k => k.Key, v => v.Value);

        var history = ReleasePressure(valves, 30, new List<List<Valve>>());

        Console.Read();
    }

    private static List<List<Valve>> ReleasePressure(Dictionary<string, Valve> valves, int minutes, List<List<Valve>> history)
    {
        List<Valve> open = new() { valves["AA"] };

        for (var i = 2; i <= minutes; i = open.Count)
        {
            foreach (var id in open.Last().Valves)
            {
                var pending = valves.Values.Except(open).ToArray();

                if (open.All(a => a.Id != id))
                {
                    open.Add(valves[id]);
                }
                else if (pending.Any())
                {
                    foreach (var to in pending)
                    {
                        var route = AddValvesInRoute(valves, open, to);
                        
                        if (route.Count > 0)
                        {
                            open.AddRange(route);
                            break;
                        }
                    }
                }
            }
        }

        history.Add(open);

        return history;
    }

    private static List<Valve> AddValvesInRoute(Dictionary<string, Valve> valves, List<Valve> open, Valve end)
    {
        List<Valve> route = new();
        
        var start = open.Last();

        while (true)
        {
            if (IsValveConnected(start, end))
            {
                route.Add(end);
                break;
            }

        }

        route.Reverse();

        return route;
    }

    private static bool IsValveConnected(Valve from, Valve to)
    {
        return from.Valves.Any(a => a == to.Id);
    }
    
    // private static bool GetConnectedValve(Valve from, Valve[] pending, out string next)
    // {
    //     next = pending.Select(valve =>
    //         from.Valves.Intersect(valve.Valves).FirstOrDefault())
    //         .FirstOrDefault(x => x != null) ?? string.Empty;
    //
    //     return !string.IsNullOrEmpty(next);
    // }
}