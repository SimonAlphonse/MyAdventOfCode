namespace TreetopTreeHouse
{
    internal class Program
    {
        record TreeHeights(int Height, int[] North, int[] West, int[] South, int[] East);

        static void Main()
        {
            var inputs = File.ReadAllLines($"inputs.txt");
            var trees = inputs.Select(s => s.Select(x => int.Parse(x.ToString())).ToList()).ToList();

            Console.WriteLine($"Part One : {GetVisibleTreesCount(trees)}");
            Console.WriteLine($"Part Two : {GetMaxScenicScore(trees)}");

            Console.Read();
        }

        private static int GetVisibleTreesCount(List<List<int>> trees)
        {
            return 2 * trees.First().Count + 2 * (trees.Count - 2) +
                   GetTreeHeightsByDirection(trees)
                       .Count(c => c.Height > c.North.Max()
                                   || c.Height > c.West.Max()
                                   || c.Height > c.South.Max()
                                   || c.Height > c.East.Max());
        }

        private static List<TreeHeights> GetTreeHeightsByDirection(List<List<int>> trees)
        {
            List<TreeHeights> treeHeights = new();
            
            for (int row = 1; row < trees.Count - 1; row++)
            {
                for (int column = 1; column < trees.First().Count - 1; column++)
                {
                    var horizontal = trees.Skip(row).First();
                    var vertical = trees.Select(s => s.Skip(column).First()).ToList();
                    var tree = horizontal.Skip(column).First();

                    treeHeights.Add(new(tree,
                        vertical.Take(row).ToArray(),
                        horizontal.Take(column).ToArray(),
                        vertical.Skip(row + 1).ToArray(),
                        horizontal.Skip(column + 1).ToArray()));
                }
            }

            return treeHeights;
        }

        private static double GetMaxScenicScore(List<List<int>> trees)
        {
            return GetTreeHeightsByDirection(trees).Select(s =>
                GetScenicScore(s.Height, s.North.Reverse().ToArray()) *
                GetScenicScore(s.Height, s.West.Reverse().ToArray()) *
                GetScenicScore(s.Height, s.South) *
                GetScenicScore(s.Height, s.East)).Max();
        }

        private static int GetScenicScore(int height, int[] trees)
        {
            List<int> visible = trees.Take(1).ToList();

            for (int i = 1; i < trees.Count(); i++)
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