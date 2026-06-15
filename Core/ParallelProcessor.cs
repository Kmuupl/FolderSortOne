using System.Diagnostics;

namespace AmdahlExperiment.Core;

public static class ParallelProcessor
{
    public static BenchmarkResult Run(string filePath, int numWorkers)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");
        if (numWorkers <= 0)
            throw new ArgumentException("Number of workers must be greater than 0.", nameof(numWorkers));

        string[] lines = File.ReadAllLines(filePath);
        int[] data = new int[lines.Length];
        for (int i = 0; i < lines.Length; i++)
            data[i] = int.Parse(lines[i]);

        string datasetName = Path.GetFileNameWithoutExtension(filePath);
        string tempDir = Path.Combine("Temp", datasetName);

        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, true);

        Stopwatch splitTimer = Stopwatch.StartNew();
        string[] chunkPaths = FileSplitter.SplitToFiles(data, numWorkers, tempDir);
        splitTimer.Stop();

        Stopwatch sortTimer = Stopwatch.StartNew();

        Thread[] threads = new Thread[numWorkers];
        for (int i = 0; i < numWorkers; i++)
        {
            string chunkPath = chunkPaths[i];
            threads[i] = new Thread(() =>
            {
                string[] chunkLines = File.ReadAllLines(chunkPath);
                int[] chunk = new int[chunkLines.Length];
                for (int j = 0; j < chunkLines.Length; j++)
                    chunk[j] = int.Parse(chunkLines[j]);

                SelectionSorter.Sort(chunk);

                using StreamWriter writer = new StreamWriter(chunkPath);
                foreach (int num in chunk)
                    writer.WriteLine(num);
            });
            threads[i].Start();
        }

        foreach (Thread t in threads)
            t.Join();

        sortTimer.Stop();

        Stopwatch mergeTimer = Stopwatch.StartNew();
        string resultPath = Path.Combine(tempDir, "result.txt");
        Merger.MergeFiles(chunkPaths, resultPath);
        mergeTimer.Stop();

        long total = splitTimer.ElapsedMilliseconds +
                     sortTimer.ElapsedMilliseconds +
                     mergeTimer.ElapsedMilliseconds;

        Console.WriteLine($"Parallel W={numWorkers} | Split: {splitTimer.ElapsedMilliseconds} ms | " +
                          $"Sort: {sortTimer.ElapsedMilliseconds} ms | " +
                          $"Merge: {mergeTimer.ElapsedMilliseconds} ms | " +
                          $"Total: {total} ms");

        return new BenchmarkResult
        {
            Workers = numWorkers,
            TotalTimeMs = total,
            SplitTimeMs = splitTimer.ElapsedMilliseconds,
            SortTimeMs = sortTimer.ElapsedMilliseconds,
            MergeTimeMs = mergeTimer.ElapsedMilliseconds,
            Speedup = 0
        };
    }
}