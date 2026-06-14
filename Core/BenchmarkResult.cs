namespace AmdahlExperiment.Core;

public class BenchmarkResult
{
    public int Workers { get; set; }
    public long TotalTimeMs { get; set; }
    public long SplitTimeMs { get; set; }
    public long SortTimeMs { get; set; }
    public long MergeTimeMs { get; set; }
    public double Speedup { get; set; }
    public double ParallelFraction { get; set; }
    public double AmdahlPrediction { get; set; }
}