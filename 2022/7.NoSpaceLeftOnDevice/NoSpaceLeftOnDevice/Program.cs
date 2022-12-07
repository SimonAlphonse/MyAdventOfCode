using System.Linq;
using System.Security;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Program
{
    record MyFile(string Name, double Size);

    record MyDirectory(string Name, List<MyDirectory> Directories, List<MyFile> Files);

    public static void Main()
    {
        var inputs = File.ReadAllLines($"inputs.txt");

        MyDirectory rootDirectory = new("~", new(), new());
        MyDirectory currentDirectory = rootDirectory;

        for (int i = 1; i < inputs.Length;)
        {
            var line = inputs.Skip(i).First();

            List<string> section;

            if (line == "$ cd ..")
            {
                if (currentDirectory.Name != "~")
                    currentDirectory = GetAllDirectories(rootDirectory)
                        .First(w => w.Directories.Any(a => a.Name == currentDirectory.Name));
                i++;
                continue;
            }
            else if (line.StartsWith("$ ls"))
            {
                section = inputs.Skip(i + 1).TakeWhile(w => !w.StartsWith('$')).ToList();
                i += section.Count + 1;
            }
            else
            {
                var directoryName = line.Split(" ").Last();
                currentDirectory = GetAllDirectories(rootDirectory)
                    .First(x => x.Name == $@"{currentDirectory.Name}\{directoryName}");
                section = inputs.Skip(i + 2).TakeWhile(w => !w.StartsWith('$')).ToList();

                i += section.Count + 2;
            }

            foreach (var file in section.Where(w => !w.Contains("dir"))
                         .Select(s => new MyFile(s.Split(" ").Last(),
                             double.Parse(s.Split(" ").First()))))
            {
                if (currentDirectory.Files.All(x => x.Name != file.Name))
                    currentDirectory.Files.Add(file);
            }

            foreach (var directory in section.Where(w => w.StartsWith("dir"))
                         .Select(s => new MyDirectory($@"{currentDirectory.Name}\{s.Split(" ").Last()}", new(), new())))
            {
                if (directory.Directories.All(a => a.Name != directory.Name))
                    currentDirectory.Directories.Add(directory);
            }
        }

        var directoryAndSpace = new Dictionary<MyDirectory, double>();

        foreach (var directory in GetAllDirectories(rootDirectory))
        {
            var allFiles = GetAllFiles(directory);
            directoryAndSpace.Add(directory, allFiles.Sum(s => s.Size));
        }

        Console.WriteLine($"Part One : {directoryAndSpace.Where(w => w.Value <= 100000).Sum(s => s.Value)}");

        var usedSpace = GetAllFiles(rootDirectory).Sum(s => s.Size);

        Console.WriteLine($"Part Two : {directoryAndSpace.OrderBy(o => o.Value).ToList()
            .First(directory => 70000000 - usedSpace + directory.Value >= 30000000).Value}");

        Console.Read();
    }

    private static List<MyDirectory> GetAllDirectories(MyDirectory directory)
    {
        var subDirectories = new List<MyDirectory>() { directory };

        foreach (var currentDirectory in directory.Directories)
            subDirectories.AddRange(GetAllDirectories(currentDirectory));

        return subDirectories.GroupBy(g => g.Name).Select(s => s.First()).ToList();
    }
    
    private static List<MyFile> GetAllFiles(MyDirectory directory)
    {
        var files = new List<MyFile>(directory.Files);

        foreach (var currentDirectory in directory.Directories)
            files.AddRange(GetAllFiles(currentDirectory));

        return files.GroupBy(g => (g.Name, g.Size)).Select(s => s.First()).ToList();
    }
}