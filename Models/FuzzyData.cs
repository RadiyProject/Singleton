using Singleton.Models.MembershipFunctions;

namespace Singleton.Models;

public class FuzzyData
{
    public static Dictionary<string, float[,]> NormalizedDataToFuzzyData(Dictionary<string, float[]> data, MembershipFunction function) 
    {
        Dictionary<string, float[,]> result = [];
        foreach (KeyValuePair<string, float[]> column in data) {
            float[,] fuzzyArray = new float[column.Value.Length, function.AreasCount + 1];
            for (int i = 0; i < column.Value.Length; i++) {
                fuzzyArray[i, 0] = column.Value[i];
                for (int j = 0; j < function.AreasCount; j++)
                    fuzzyArray[i, 1 + j] = function.CalculateMembershipValue(column.Value[i], j);
            }

            result[column.Key] = fuzzyArray;
        }

        return result;
    }

    public static Dictionary<string, float[]> GenerateRules(Dictionary<string, float[,]> fuzzificatedInput, float[] distinctOutput)
    {
        int areasCount = fuzzificatedInput.First().Value.GetLength(1) - 1;
        int rulesCount = (int)Math.Pow(areasCount, fuzzificatedInput.Keys.Count);
        if(distinctOutput.Length > rulesCount)
            throw new InvalidDataException("Количество категорий выходной переменной превышает количество правил. Попробуйте увеличить количество термов.");

        Dictionary<string, float[]> rules = [];
        float[] rule;

        int i = 0;
        foreach (KeyValuePair<string, float[,]> column in fuzzificatedInput) {
            rule = new float[rulesCount];
            int step = (int)Math.Pow(areasCount, i);
            int areaId = -1;
            for (int j = 0; j < rulesCount; j++)
            {
                if (j % step == 0) {
                    areaId++;
                    if (areaId >= areasCount)
                        areaId = 0;
                }

                rule[j] = areaId;
            }
            rules[column.Key] = rule;

            i++;
        }

        int[] bag = new int[distinctOutput.Length];
        int repeatedCount = rulesCount / distinctOutput.Length;
        int idx = new Random().Next(0, bag.Length);
        for (i = 0; i < bag.Length; i++)
        {
            if (i != idx)
                bag[i] = repeatedCount;
            else
                bag[i] = repeatedCount + rulesCount % distinctOutput.Length;

        }

        rule = new float[rulesCount];
        for (i = 0; i < rule.Length; i++)
        {
            do {
                idx = new Random().Next(0, bag.Length);
            } while (bag[idx] <= 0);

            rule[i] = distinctOutput[idx];
            bag[idx]--;

        }
        
        rules["output"] = rule;

        return rules;
    }

    //Разделять данные на тестовую и обучающую выборки
}
