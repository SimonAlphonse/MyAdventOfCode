namespace TreetopTreeHouse
{
    internal class Program
    {
        record TreeHeights(int Height, int[] North, int[] West, int[] South, int[] East);

        static void Main()
        {
            var inputs = File.ReadAllLines($"inputs.txt");
            var trees = inputs.Select(s => s.Select(x => int.Parse(x.ToString())).ToArray()).ToList();

            Console.WriteLine($"Part One : {GetVisibleTreesCount(trees)}");
            Console.WriteLine($"Part Two : {GetMaxScenicScore(trees)}");

            Console.Read();
        }

        private static int GetVisibleTreesCount(List<int[]> trees)
        {
            return 2 * trees.First().Length + 2 * (trees.Count - 2) +
                   GetTreeHeightsByDirection(trees).Count(c
                       => c.Height > c.North.Max() || c.Height > c.West.Max() ||
                          c.Height > c.South.Max() || c.Height > c.East.Max());
        }

        private static List<TreeHeights> GetTreeHeightsByDirection(List<int[]> trees)
        {
            List<TreeHeights> treeHeights = new();
            
            for (var row = 1; row < trees.Count - 1; row++)
            {
                for (var column = 1; column < trees.First().Length - 1; column++)
                {
                    treeHeights.Add(new(trees[row][column],
                        trees.Select(r => r[column]).Take(row).ToArray(),
                        trees[row].Take(column).ToArray(),
                        trees.Select(r => r[column]).Skip(row + 1).ToArray(),
                        trees[row].Skip(column + 1).ToArray()));
                }
            }

            return treeHeights;
        }

        private static double GetMaxScenicScore(List<int[]> trees)
        {
            return GetTreeHeightsByDirection(trees).Select(s =>
                GetScenicScore(s.Height, s.North.Reverse().ToArray()) *
                GetScenicScore(s.Height, s.West.Reverse().ToArray()) *
                GetScenicScore(s.Height, s.South) *
                GetScenicScore(s.Height, s.East)).Max();
        }

        private static int GetScenicScore(int height, int[] trees)
        {
            List<int> visible = new() { trees.First() };

            for (var i = 1; i < trees.Length; i++)
            {
                var previous = visible.Last();
                if (previous >= height) break;
                
                var current = trees.Skip(i).First();
                visible.Add(current);
                
                if (current >= height) break;
            }

            return visible.Count();
        }
    }
}