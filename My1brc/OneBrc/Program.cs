using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace OneBrc
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"{DateTime.Now:T}");
            var stopwatch = new Stopwatch(); stopwatch.Start();

            var fileName = @"C:\Users\Windows\source\repos\1brc\measurements.txt";

            var measurements = ReadMeasurements(fileName, 100 * 1024 * 1024);

            //Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds:F2} s");

            //var averageTask = Task.Run(() => Console.WriteLine($"{measurements.Average(x => x.Temperature):F4}"));
            //var minTask = Task.Run(() => Console.WriteLine($"{measurements.Min(x => x.Temperature):F4}"));
            //var maxTask = Task.Run(() => Console.WriteLine($"{measurements.Max(x => x.Temperature):F4}"));
            //Task.WaitAll(averageTask, minTask, maxTask);

            Console.WriteLine($"{DateTime.Now:T} | Time Taken : {stopwatch.Elapsed.TotalSeconds:F2} s");
            Console.ReadKey();
        }

        public static Measurement[] ReadMeasurements(string fileName, long chunkSize)
        {
            var allMeasurements = new ConcurrentBag<Measurement[]>();

            long total = new FileInfo(fileName).Length;

            using var mmf = MemoryMappedFile.CreateFromFile(fileName, FileMode.Open, "1Brc");
            {
                var posisions = GetPosisions(mmf, total, chunkSize);

                ParallelOptions parallelOptions = new() { MaxDegreeOfParallelism = Environment.ProcessorCount };

                Parallel.For(0, posisions.Length, parallelOptions, i =>
                {
                    var measurements = ReadMeasurements(mmf, posisions[i]);
                    allMeasurements.Add(measurements.ToArray());
                });
            }

            return allMeasurements.SelectMany(s => s).ToArray();
        }

        private static Position[] GetPosisions(MemoryMappedFile mmf, long total, long chunk)
        {
            using var stream = mmf.CreateViewStream(0, total);
            var bytes = new byte[1];
            List<Position> positions = [];

            do
            {
                var last = positions.LastOrDefault();
                long start = last?.Start + last?.Length + 1 ?? 0;
                if (start > total) { break; }

                stream.Seek(chunk, SeekOrigin.Current);
                do stream.Read(bytes, 0, 1);
                while (bytes[0] != '\n' && bytes[0] != 0);

                positions.Add(new(start, Math.Min(stream.Position, total) - start));
            } while (true);

            return positions.ToArray();
        }

        private static List<Measurement> ReadMeasurements(MemoryMappedFile mmf, Position position)
        {
            using MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor(position.Start, position.Length);

            List<Measurement> measurements = [];

            int i = 0; int length = 0;
            var bytes = new byte[1];
            byte[] byteCity = default;
            byte[] byteTemperature = default;

            while (i < accessor.Capacity)
            {
                if (accessor.ReadByte(i) == ';')
                {
                    byteCity = new byte[length];
                    accessor.ReadArray(i - length, byteCity, 0, length);
                    length = 0;
                }
                else if (accessor.ReadByte(i) == '\n')
                {
                    byteTemperature = new byte[length];
                    accessor.ReadArray(i - length, byteTemperature, 0, length);
                    measurements.Add(new(byteCity, float.Parse(byteTemperature)));
                    length = 0;
                }
                else { length++; }

                i++;
            }

            return measurements;
        }
    }

    public record Position(long Start, long Length);

    public struct Measurement(byte[] city, float temperature)
    {
        public byte[] City = city;
        public float Temperature = temperature;

        //public readonly string GetCity() => Encoding.UTF8.GetString(city);
    }
}