namespace AmdahlExperiment.Core;

public static class FileGenerator
{
    public static void GenerateAll()
    {
        Directory.CreateDirectory("Data");
        Directory.CreateDirectory("Results");

        Generate("Data/small.txt", 10_000);
        Generate("Data/medium.txt", 50_000);
        Generate("Data/large.txt", 150_000);
    }

    private static void Generate(string filePath, int count)
    {
        if (File.Exists(filePath))
        {
            Console.WriteLine($"Already exists: {filePath}");
            return;
        }

        Random random = new Random();

        using StreamWriter writer = new StreamWriter(filePath);
        for (int i = 0; i < count; i++)
        {
            writer.WriteLine(random.Next(1, 100_000));
        }

        Console.WriteLine($"Generated: {filePath} ({count} numbers)");
    }
}