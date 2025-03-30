using Csv;

namespace Singleton.Models;

public class Dataset {

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="keys"></param>
    /// <param name="rowsCount"></param>
    /// <returns></returns>
    public static async Task<Dictionary<string, List<string>>> ReadDataset(string path, string[] keys, int? rowsCount = null) {
        string? csv = File.ReadAllText(path);
        Dictionary<string, List<string>> result = [];

        int i = 0;
        await foreach (ICsvLine line in CsvReader.ReadFromTextAsync(csv))
        {
            if (rowsCount != null && rowsCount <= i)
                break;

            foreach (string key in keys)
                result[key] = result.TryGetValue(key, out List<string>? value) 
                    ? [.. value, line[key]] : result[key] = [line[key]];
            i++;
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static List<float> TextTofloat(List<string> values) {
        List<string> distincts = [.. values.Distinct()];
        Dictionary<string, int> labels = [];
        int i = 0;
        foreach (string distinct in distincts) {
            labels[distinct] = i + 1;
            i++;
        }

        List<float> numbers = [];
        foreach (string value in values)
            numbers = [.. numbers, labels[value]];

        return numbers;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static List<float>? StringifiedNumbersTofloat(List<string> values) {
        List<float> numbers = [];
        foreach (string value in values)
            if (float.TryParse(value, out float parsed))
                numbers = [.. numbers, parsed];
            else
                return null;

        return numbers;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public static float[] Normalize(float[] values) {
        float min = values[0];
        float max = values[0];
        foreach (float value in values) {
            if (value < min)
                min = value;
            if (value > max)
                max = value;
        }

        float subtracted = max - min;
        for (int i = 0; i < values.Length; i++)
            values[i] = subtracted != 0 ? (values[i] - min) / subtracted : 1;

        return values;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataset"></param>
    /// <returns></returns>
    public static Dictionary<string, float[]> ConvertDatasetToNormalizedData(Dictionary<string, List<string>> dataset) {
        Dictionary<string, float[]> result = [];
        foreach (KeyValuePair<string, List<string>> column in dataset) {
            List<float>? convertedColumn = StringifiedNumbersTofloat(column.Value);
            convertedColumn ??= TextTofloat(column.Value);

            result[column.Key] = Normalize([.. convertedColumn]);
        }

        return result;
    }

    public static Dictionary<string, float[]> ConvertOutputToCategories(List<string> values) {
        Dictionary<string, float[]> result = [];
        List<float>? convertedColumns = TextTofloat(values);
        float[] normalized = Normalize([.. convertedColumns]);

        int i = 0;
        foreach (string column in values) {
            result[column] = [normalized[i]];
            i++;
        }

        return result;
    }

    public static Dictionary<string, float[]>[] Separate(Dictionary<string, float[]> dataset, float trainProportion) {
        if (trainProportion > 1 || trainProportion <= 0)
            throw new InvalidDataException("Доля обучающей выборки задана некорректно");

        int datasetCount = dataset.Values.First().Length;
        int trainCount = (int)(datasetCount * trainProportion);
        int testCount = datasetCount - trainCount;
        Dictionary<string, float[]>[] result = new Dictionary<string, float[]>[2];
        Dictionary<string, List<float>>[] temp = new Dictionary<string, List<float>>[2];
        float probability = (float)trainCount / testCount;
        int idx = 0;
        while (trainCount > 0 || testCount > 0) {
            int set;
            if (trainCount > 0 && testCount > 0) {
                set = new Random().Next(0, (int)(2 * probability));
                if (set > 0)
                    set = 0;
                else
                    set = 1;

                if (probability < 1)
                    set = set == 1 ? 0 : 1;
            }
            else if (trainCount > 0)
                set = 0;
            else
                set = 1;

            CopyRow(dataset, idx, ref temp[set]);
            if (set == 0) 
                trainCount--;
            else
                testCount--;

            idx++;
        }

        for(int i = 0; i < result.Length; i++)
        {
            result[i] ??= [];

            foreach(KeyValuePair<string, List<float>> column in temp[i])
                result[i][column.Key] = [.. column.Value];
        }

        return result;
    }

    private static void CopyRow(Dictionary<string, float[]> dataset, int idx, ref Dictionary<string, List<float>> set) {
        set ??= [];

        foreach(KeyValuePair<string, float[]> column in dataset) {
            if (!set.ContainsKey(column.Key))
                set[column.Key] = [];

            set[column.Key].Add(column.Value[idx]);
        }
    }

    public static Dictionary<string, float[]> Reduce(Dictionary<string, float[]> dataset, float reduceValue) {
        if (reduceValue >= 1 || reduceValue <= 0)
            throw new InvalidDataException("Коэффициент для сокращения датасета указан неверно");

        int datasetCount = dataset.Values.First().Length;
        int resultCount = (int)(datasetCount * reduceValue);
        int step = datasetCount / resultCount;
        Dictionary<string, float[]> result = [];
        Dictionary<string, List<float>> temp = [];
        for (int i = 0; i < datasetCount; i += step)
            CopyRow(dataset, i, ref temp);

        foreach(KeyValuePair<string, List<float>> column in temp)
            result[column.Key] = [.. column.Value];

        return result;
    }
}