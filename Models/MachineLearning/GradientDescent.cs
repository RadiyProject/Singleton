using Singleton.Models.MembershipFunctions;

namespace Singleton.Models.MachineLearning;

public class GradientDescent
{
    public Dictionary<string, float[]> Train(Dictionary<string, float[]> rules, Dictionary<string, float[]> weights, 
        Dictionary<string, float[]> dataset, MembershipFunction function, string outputName, 
        int epochsCount = 5) 
    {
        for (int epoch = 0; epoch < epochsCount; epoch++) {
            for (int row = 0; row < dataset.Values.First().Length; row++) {
                var singleton = new OutputFunctions.Singleton(rules, function, 1, weights);
                float[] wj = new float[rules.Values.First().Length];
                for (int i = 0; i < wj.Length; i++) {
                    Dictionary<string, float> input = [];
                    foreach(KeyValuePair<string, float[]> column in dataset)
                        input[column.Key == outputName ? "output" : column.Key] = column.Value[row];
                    wj[i] = singleton.GetDegree(input, i);
                }
            }
        }

        return weights;
    }
}