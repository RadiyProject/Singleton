using Csv;

namespace Singleton.Models;

public class Dataset {
    public static Dictionary<string, List<string>> ReadDataset(string path, string[] keys) {
        string? csv = File.ReadAllText(path);
        Dictionary<string, List<string>> result = [];

        foreach (ICsvLine line in CsvReader.ReadFromText(csv))
        {
            foreach (string key in keys) {
                result[key] = result.TryGetValue(key, out List<string>? value) 
                    ? [.. value, line[key]] : result[key] = [line[key]];
            }
        }

        return result;
    }

    public static List<double> TextToDouble(List<string> values) {
        List<string> distincts = [.. values.Distinct()];
        Dictionary<string, int> labels = [];
        int i = 0;
        foreach (string distinct in distincts) {
            labels[distinct] = i;
            i++;
        }

        List<double> numbers = [];
        foreach (string value in values)
            numbers = [.. numbers, labels[value]];

        return numbers;
    }

    public static List<double>? StringifiedNumbersToDouble(List<string> values) {
        List<double> numbers = [];
        foreach (string value in values)
            if (double.TryParse(value, out double parsed))
                numbers = [.. numbers, parsed];
            else
                return null;

        return numbers;
    }

    public static double[] Normalize(double[] values) {
        double min = values[0];
        double max = values[0];
        foreach (double value in values) {
            if (value < min)
                min = value;
            if (value > max)
                max = value;
        }

        double subtracted = max - min;
        for (int i = 0; i < values.Length; i++)
            values[i] = (values[i] - min) / subtracted;

        return values;
    }
}