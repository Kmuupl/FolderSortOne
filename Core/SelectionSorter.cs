namespace AmdahlExperiment.Core;

public static class SelectionSorter
{
    public static void Sort(int[] array)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));

        int len = array.Length;

        for (int i = 0; i < len - 1; i++)
        {
            int minIndex = i;

            for (int j = i + 1; j < len; j++)
            {
                if (array[j] < array[minIndex])
                    minIndex = j;
            }

            if (minIndex != i)
            {
                int temp = array[i];
                array[i] = array[minIndex];
                array[minIndex] = temp;
            }
        }
    }
}