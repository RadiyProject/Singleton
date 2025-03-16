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
}