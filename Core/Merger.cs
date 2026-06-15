namespace AmdahlExperiment.Core;

public static class Merger
{
    public static void MergeFiles(string[] chunkPaths, string outputPath)
    {
        if (chunkPaths == null)
            throw new ArgumentNullException(nameof(chunkPaths));

        int[][] chunks = new int[chunkPaths.Length][];
        for (int i = 0; i < chunkPaths.Length; i++)
        {
            string[] lines = File.ReadAllLines(chunkPaths[i]);
            int[] numbers = new int[lines.Length];
            for (int j = 0; j < lines.Length; j++)
                numbers[j] = int.Parse(lines[j]);
            chunks[i] = numbers;
        }

        int[] result = chunks[0];
        for (int i = 1; i < chunks.Length; i++)
        {
            result = MergeTwo(result, chunks[i]);
        }

        using StreamWriter writer = new StreamWriter(outputPath);
        foreach (int num in result)
        {
            writer.WriteLine(num);
        }
    }

    private static int[] MergeTwo(int[] left, int[] right)
    {
        int[] merged = new int[left.Length + right.Length];
        int i = 0, j = 0, k = 0;

        while (i < left.Length && j < right.Length)
        {
            if (left[i] <= right[j])
                merged[k++] = left[i++];
            else
                merged[k++] = right[j++];
        }

        while (i < left.Length)
            merged[k++] = left[i++];

        while (j < right.Length)
            merged[k++] = right[j++];

        return merged;
    }
}