using System.Diagnostics;
using System.Globalization;

namespace AmdahlExperiment.Core;

public static class BenchmarkRunner
{
    public static void Run(string filePath, int maxWorkers)
    {
        Console.WriteLine($"\n=== Benchmark: {filePath} ===");

        List<BenchmarkResult> results = new List<BenchmarkResult>();

        // читаем данные один раз
        string[] lines = File.ReadAllLines(filePath);
        int[] data = new int[lines.Length];
        for (int i = 0; i < lines.Length; i++)
            data[i] = int.Parse(lines[i]);

        // P=1 — сортируем весь массив целиком
        Stopwatch sw = Stopwatch.StartNew();
        SelectionSorter.Sort((int[])data.Clone());
        sw.Stop();
        long t1 = sw.ElapsedMilliseconds;

        Console.WriteLine($"Sequential | {filePath} | Time: {t1} ms");

        results.Add(new BenchmarkResult
        {
            Workers = 1,
            TotalTimeMs = t1,
            SplitTimeMs = 0,
            SortTimeMs = t1,
            MergeTimeMs = 0,
            Speedup = 1.0,
            ParallelFraction = 0,
            AmdahlPrediction = 1.0
        });

        // P=2,3,4... — параллельно
        double f = 0; // будет вычислена из первого параллельного запуска

        for (int p = 2; p <= maxWorkers; p++)
        {
            BenchmarkResult result = ParallelProcessor.Run(filePath, p);

            result.Speedup = t1 > 0 ? (double)t1 / result.TotalTimeMs : 0;

            // f вычисляем только один раз из P=2
            if (p == 2)
            {
                f = AmdahlCalculator.CalculateParallelFraction(
                    result.SortTimeMs,
                    result.TotalTimeMs
                );
            }

            result.ParallelFraction = f;
            result.AmdahlPrediction = AmdahlCalculator.CalculateAmdahlSpeedup(f, p);

            results.Add(result);
        }

        // после цикла обновляем P=1
        results[0].ParallelFraction = f;
        results[0].AmdahlPrediction = 1.0;

        SaveToCsv(filePath, results);
        PrintSummary(results);
    }

    private static void SaveToCsv(string filePath, List<BenchmarkResult> results)
    {
        string datasetName = Path.GetFileNameWithoutExtension(filePath).ToLower();
        string csvPath = Path.Combine("Results", $"{datasetName}_benchmark.csv");

        Directory.CreateDirectory("Results");

        using StreamWriter writer = new StreamWriter(csvPath);
        writer.WriteLine("Workers;TotalTimeMs;SplitTimeMs;SortTimeMs;MergeTimeMs;Speedup;ParallelFraction;AmdahlPrediction");

        foreach (BenchmarkResult r in results)
        {
            writer.WriteLine(
                $"{r.Workers};" +
                $"{r.TotalTimeMs};" +
                $"{r.SplitTimeMs};" +
                $"{r.SortTimeMs};" +
                $"{r.MergeTimeMs};" +
                $"{r.Speedup.ToString("F2", CultureInfo.InvariantCulture)};" +
                $"{r.ParallelFraction.ToString("F3", CultureInfo.InvariantCulture)};" +
                $"{r.AmdahlPrediction.ToString("F2", CultureInfo.InvariantCulture)}"
            );
        }

        Console.WriteLine($"Saved: {csvPath}");
    }

    private static void PrintSummary(List<BenchmarkResult> results)
    {
        Console.WriteLine("\nWorkers | Total ms | Split ms | Sort ms | Merge ms | Speedup | Amdahl");
        Console.WriteLine(new string('-', 75));

        foreach (BenchmarkResult r in results)
        {
            Console.WriteLine(
                $"{r.Workers,7} | {r.TotalTimeMs,8} | {r.SplitTimeMs,8} | " +
                $"{r.SortTimeMs,7} | {r.MergeTimeMs,8} | " +
                $"{r.Speedup,7:F2} | {r.AmdahlPrediction,6:F2}"
            );
        }
    }
}