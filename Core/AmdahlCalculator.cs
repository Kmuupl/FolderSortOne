namespace AmdahlExperiment.Core;

public static class AmdahlCalculator
{
    public static double CalculateParallelFraction(long sortTimeMs, long totalTimeMs)
    {
        if (totalTimeMs == 0) return 0;
        return (double)sortTimeMs / totalTimeMs;
    }

    public static double CalculateAmdahlSpeedup(double f, int workers)
    {
        if (workers <= 0) return 1.0;
        return 1.0 / ((1.0 - f) + f / workers);
    }

    public static double CalculateTheoreticalMax(double f)
    {
        if (f >= 1.0) return double.PositiveInfinity;
        return 1.0 / (1.0 - f);
    }
}