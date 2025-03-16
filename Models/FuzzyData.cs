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

    public static Dictionary<string, int[]> GenerateRules(Dictionary<string, float[,]> fuzzificatedInput, float[] distinctOutput)
    {
        int areasCount = fuzzificatedInput.First().Value.Length - 1;
        int rulesCount = (int)Math.Pow(areasCount, fuzzificatedInput.Count);
        if(distinctOutput.Length > rulesCount)
            throw new InvalidDataException("Количество категорий выходной переменной превышает количество правил. Попробуйте увеличить количество термов.");

        Dictionary<string, int[]> rules = [];

        int i = 0;
        foreach (KeyValuePair<string, float[,]> column in fuzzificatedInput) {
            int[] rule = new int[rulesCount];
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
            rules[column.Key] = rule;//Сделать мешок значений для случайной подстановки выходного значения, где для каждого значения будет максимальное возможное количество использований (делим количество правил на количество distinctOutput)

            i++;
        }

        return rules;
    }

    //Разделять данные на тестовую и обучающую выборки
}
