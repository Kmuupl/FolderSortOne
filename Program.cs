using AmdahlExperiment.Core;

int maxWorkers = Environment.ProcessorCount;
Console.WriteLine($"CPU cores detected: {maxWorkers}");

Console.WriteLine("\nGenerating files...");
FileGenerator.GenerateAll();
Console.WriteLine("Done!\n");

BenchmarkRunner.Run("Data/small.txt",  maxWorkers);
BenchmarkRunner.Run("Data/medium.txt", maxWorkers);
BenchmarkRunner.Run("Data/large.txt",  maxWorkers);

Console.WriteLine("\nAll benchmarks complete!");
Console.WriteLine("Check Results/ folder for CSV files.");