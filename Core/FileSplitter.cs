namespace AmdahlExperiment.Core;

public static class FileSplitter
{
    public static string[] SplitToFiles(int[] data, int parts, string tempDir)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        if (parts <= 0)
            throw new ArgumentException("Parts must be greater than 0.", nameof(parts));

        Directory.CreateDirectory(tempDir);

        string[] chunkPaths = new string[parts];
        int chunkSize = data.Length / parts;

        for (int i = 0; i < parts; i++)
        {
            int start = i * chunkSize;
            int end = (i == parts - 1) ? data.Length : start + chunkSize;

            string chunkPath = Path.Combine(tempDir, $"chunk_{i}.txt");
            chunkPaths[i] = chunkPath;

            using StreamWriter writer = new StreamWriter(chunkPath);
            for (int j = start; j < end; j++)
            {
                writer.WriteLine(data[j]);
            }
        }

        return chunkPaths;
    }
}