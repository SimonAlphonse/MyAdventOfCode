namespace RockPaperScissors;

public abstract class Program
{
    private record GameOne(char Opponent, char Me);
    private record GameTwo(char Opponent, char Result);

    public static void Main(string[] args)
    {
        var lines = File.ReadAllLines("inputs.txt");
        Console.WriteLine($"Part One : {GetScore(lines.Select(s => new GameOne(s.First(), s.Last())))}");
        Console.WriteLine($"Part Two : {GetScore(lines.Select(s => new GameTwo(s.First(), s.Last())))}");
        Console.Read();
    }

    private static int GetScore(IEnumerable<GameOne> games)
    {
        return games.Sum(game => game switch
        {
            { Opponent: 'B', Me: 'X' } => 1,
            { Opponent: 'C', Me: 'Y' } => 2,
            { Opponent: 'A', Me: 'Z' } => 3,
            { Opponent: 'A', Me: 'X' } => 4,
            { Opponent: 'B', Me: 'Y' } => 5,
            { Opponent: 'C', Me: 'Z' } => 6,
            { Opponent: 'C', Me: 'X' } => 7,
            { Opponent: 'A', Me: 'Y' } => 8,
            { Opponent: 'B', Me: 'Z' } => 9,
            _ => throw new InvalidDataException($"{game}")
        });
    }
    
    private static int GetScore(IEnumerable<GameTwo> games)
    {
        List<GameOne> gameOnes = (from game in games
            let you = game switch
            {
                { Opponent: 'A', Result: 'X' } => 'Z',
                { Opponent: 'B', Result: 'X' } => 'X',
                { Opponent: 'C', Result: 'X' } => 'Y',
                { Opponent: 'A', Result: 'Y' } => 'X',
                { Opponent: 'B', Result: 'Y' } => 'Y',
                { Opponent: 'C', Result: 'Y' } => 'Z',
                { Opponent: 'A', Result: 'Z' } => 'Y',
                { Opponent: 'B', Result: 'Z' } => 'Z',
                { Opponent: 'C', Result: 'Z' } => 'X',
                _ => throw new InvalidDataException($"{game}")
            }
            select new GameOne(game.Opponent, you)).ToList();

        return GetScore(gameOnes);
    }
}