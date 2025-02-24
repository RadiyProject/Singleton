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

    public static List<int> TextToNumber(List<string> values) {
        List<string> distincts = [.. values.Distinct()];
        Dictionary<string, int> labels = [];
        int i = 0;
        foreach (string distinct in distincts) {
            labels[distinct] = i;
            i++;
        }

        List<int> numbers = [];
        foreach (string value in values)
            numbers = [.. numbers, labels[value]];

        return numbers;
    }
}