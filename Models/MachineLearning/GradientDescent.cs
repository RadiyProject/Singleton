using Singleton.Models.MachineLearning.AccuracyEstimators;
using Singleton.Models.MachineLearning.Deviations;
using Singleton.Models.MembershipFunctions;

namespace Singleton.Models.MachineLearning;

public class GradientDescent
{
    public Dictionary<string, float[]> Train(Dictionary<string, float[]> rules, Dictionary<string, float[]> weights, 
        Dictionary<string, float[]> dataset, MembershipFunction function, string outputName, 
        int epochsCount = 5, float learnCoeff = 2) 
    {
        float learningRate = 0;
        float offsetLearningRate = 0.0001f;
        List<float> realOut = []; List<float> expectedOut = [];
        for (int epoch = 0; epoch < epochsCount; epoch++) {
            //if (epoch > 1)
            //    learningRate /= 1 + learnCoeff * epoch;
            float error = 0;
            bool isLast = epoch == epochsCount - 1;

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
                if (isLast) {
                    realOut.Add(result);
                    expectedOut.Add(dataset[outputName][row]);
                }
                float deviation = new StandardDeviation().DerivativeCalculateRow(result, dataset[outputName][row]);
                error += new StandardDeviation().CalculateRow(result, dataset[outputName][row]);
                //Console.WriteLine($"Error: {error}. Epochs: {epoch}. Row: {row}");

                //for (int i = 0; i < wj.Length; i++)
                //        weights["output"][i] -= learningRate * deviation * ((rules["output"][i] < wj[i] ? rules["output"][i] : wj[i]) / lowerMij);
                    
                for (int i = 0; i < wj.Length; i++)
                    foreach (KeyValuePair<string, float[]> rule in rules) {
                        if (rule.Key == "output")
                            continue;

                        Dictionary<string, float> input = [];
                        foreach(KeyValuePair<string, float[]> column in dataset)
                            input[column.Key == outputName ? "output" : column.Key] = column.Value[row];

                        float wjD = singleton.GetDerivated(input, i);
                        weights[rule.Key][i] -= offsetLearningRate * deviation * ((rules["output"][i] * weights["output"][i] < wjD ? rules["output"][i] * weights["output"][i] : wjD) / lowerMij);
                    }
            }
            Console.WriteLine($"Error: {error / dataset.Values.First().Length}. Epochs: {epoch}");
            if (isLast)
                Console.WriteLine($"Overall accuracy: {new DeterminationCoefficient().Calculate(realOut, expectedOut)}.");
        }

        return weights;
    }
}