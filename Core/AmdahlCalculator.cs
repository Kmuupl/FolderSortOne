namespace AmdahlExperiment.Core;

public static class AmdahlCalculator
{
    // f = доля параллельной части (Sort) в общем времени
    public static double CalculateParallelFraction(long sortTimeMs, long totalTimeMs)
    {
        if (totalTimeMs == 0) return 0;
        return (double)sortTimeMs / totalTimeMs;
    }

    // закон Амдала: S(P) = 1 / ((1 - f) + f / P)
    public static double CalculateAmdahlSpeedup(double f, int workers)
    {
        if (workers <= 0) return 1.0;
        return 1.0 / ((1.0 - f) + f / workers);
    }

    // теоретический максимум при P → ∞: S_max = 1 / (1 - f)
    public static double CalculateTheoreticalMax(double f)
    {
        if (f >= 1.0) return double.PositiveInfinity;
        return 1.0 / (1.0 - f);
    }
}