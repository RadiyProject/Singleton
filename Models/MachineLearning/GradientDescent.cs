using Singleton.Models.MachineLearning.Deviations;
using Singleton.Models.MembershipFunctions;

namespace Singleton.Models.MachineLearning;

public class GradientDescent
{
    public Dictionary<string, float[]> Train(Dictionary<string, float[]> rules, Dictionary<string, float[]> weights, 
        Dictionary<string, float[]> dataset, MembershipFunction function, string outputName, 
        int epochsCount = 5, float learnCoeff = 2) 
    {
        float learningRate = 0.5f;
        for (int epoch = 0; epoch < epochsCount; epoch++) {
            learningRate /= (1 + learnCoeff * epoch);

            for (int row = 0; row < dataset.Values.First().Length; row++) {
                var singleton = new OutputFunctions.Singleton(rules, function, 1, weights);
                float[] wj = new float[rules.Values.First().Length];
                (int, float) r = singleton.GetR();
                float lowerMij = 0;

                for (int i = 0; i < wj.Length; i++) {
                    Dictionary<string, float> input = [];
                    foreach(KeyValuePair<string, float[]> column in dataset)
                        input[column.Key == outputName ? "output" : column.Key] = column.Value[row];

                    wj[i] = singleton.GetDegree(input, i);
                    if (wj[i] > lowerMij)
                        lowerMij = wj[i];
                }
                float upperMij = (wj[r.Item1] >= r.Item2) ? r.Item2 : wj[r.Item1];

                float result = upperMij / lowerMij;
                float deviation = new StandardDeviation().CalculateRow(result, dataset[outputName][row]);
                float error = deviation * deviation;
            }
        }

        return weights;
    }
}