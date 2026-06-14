using AmdahlExperiment.Core;

int maxWorkers = Environment.ProcessorCount;
Console.WriteLine($"CPU cores detected: {maxWorkers}");

// Этап 1 — генерация файлов
Console.WriteLine("\nGenerating files...");
FileGenerator.GenerateAll();
Console.WriteLine("Done!\n");

// Этап 2 — бенчмарк
BenchmarkRunner.Run("Data/small.txt",  maxWorkers);
BenchmarkRunner.Run("Data/medium.txt", maxWorkers);
BenchmarkRunner.Run("Data/large.txt",  maxWorkers);

Console.WriteLine("\nAll benchmarks complete!");
Console.WriteLine("Check Results/ folder for CSV files.");