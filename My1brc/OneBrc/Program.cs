﻿using System.Text;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Collections.Concurrent;

namespace OneBrc
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = new Stopwatch(); stopwatch.Start();

            var fileName = @"C:\Users\Windows\source\repos\1brc\measurements.txt";

            var measurements = ReadMeasurements(fileName, 100 * 1024 * 1024);

            Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds:F2} s");
            Console.ReadKey();
        }

        public static List<Measurement> ReadMeasurements(string fileName, long chunkSize)
        {
            var allMeasurements = new ConcurrentBag<List<Measurement>>();

            long total = new FileInfo(fileName).Length;

            using var mmf = MemoryMappedFile.CreateFromFile(fileName, FileMode.Open);
            {
                var posisions = GetPosisions(mmf, total, chunkSize);

                ParallelOptions parallelOptions = new() { MaxDegreeOfParallelism = Environment.ProcessorCount };

                //for (int i = 0; i < posisions.Length; i++)
                Parallel.For(0, posisions.Length, parallelOptions, i =>
                {
                    var stopwatch = new Stopwatch(); stopwatch.Start();
                    var measurements = ReadMeasurements(mmf, posisions[i]);
                    Console.WriteLine($"{i} : {measurements.Count()} -> {stopwatch.Elapsed.TotalSeconds:F2} s");
                    allMeasurements.Add(measurements.ToList());
                });
            }

            return allMeasurements.SelectMany(s => s).ToList();
        }

        private static Position[] GetPosisions(MemoryMappedFile mmf, long total, long chunk)
        {
            using var stream = mmf.CreateViewStream(0, total);
            var bytes = new byte[1];
            List<Position> positions = [];

            do
            {
                var last = positions.LastOrDefault();
                long start = last?.End + 1 ?? 0;
                if (start > total) { break; }

                stream.Seek(chunk, SeekOrigin.Current);
                do stream.Read(bytes, 0, 1);
                while (bytes[0] != '\n' && bytes[0] != 0);

                positions.Add(new(start, Math.Min(stream.Position, total)));
            } while (true);

            return positions.ToArray();
        }

        private static IEnumerable<Measurement> ReadMeasurements(MemoryMappedFile mmf, Position position)
        {
            using MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor(position.Start, position.End - position.Start);

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
                    var city = Encoding.UTF8.GetString(byteCity);
                    length = 0;
                }
                else if (accessor.ReadByte(i) == '\n')
                {
                    byteTemperature = new byte[length];
                    accessor.ReadArray(i - length, byteTemperature, 0, length);
                    yield return new(byteCity, float.Parse(byteTemperature));
                    length = 0;
                }
                else { length++; }

                i++;
            }
        }
    }

    public record Position(long Start, long End);

    public struct Measurement(byte[] City, float Temperature)
    {
        //public readonly string GetCity() => Encoding.UTF8.GetString(City);
    }
}