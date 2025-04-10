using Singleton.Models.MachineLearning.AccuracyEstimators;
using Singleton.Models.MachineLearning.Deviations;
using Singleton.Models.MembershipFunctions;

namespace Singleton.Models.MachineLearning;

public class GradientDescent
{
    public async Task<Dictionary<string, float[]>> Train(Dictionary<string, float[]> rules, Dictionary<string, float[]> weights, 
        Dictionary<string, float[]> dataset, MembershipFunction function, string outputName, 
        int epochsCount = 5) 
    {
        float startLearningRate = 0.5f;
        float attenuationCoef = 0.9f;
        float learningRate = startLearningRate;
        List<float> realOut = []; List<float> expectedOut = [];
        int batchCount = 50;

        //float momentum = 0.9f;
        //float[] velocity = new float[weights["output"].Length];

        int autosaveCount = 50;
        for (int epoch = 0; epoch < epochsCount; epoch++) {
            float error = 0;
            //bool isLast = epoch == epochsCount - 1;

            Dictionary<string, float[]>[] batchedDataset = new Dictionary<string, float[]>[batchCount];
            Dictionary<string, List<float>>[] tempBatches = new Dictionary<string, List<float>>[batchCount];
            for (int batch = 0; batch < batchCount; batch++)
                tempBatches[batch] = [];
            
            for(int i = 0; i < dataset.First().Value.Length; i++) {
                int currentIdx = new Random().Next(0, batchCount);
                foreach(KeyValuePair<string, float[]> column in dataset) {
                    if (!tempBatches[currentIdx].ContainsKey(column.Key))
                        tempBatches[currentIdx][column.Key] = [];

                    tempBatches[currentIdx][column.Key].Add(column.Value[i]);
                }
            }
            for (int batch = 0; batch < batchCount; batch++) {
                batchedDataset[batch] = [];

                foreach(KeyValuePair<string, List<float>> column in tempBatches[batch]) 
                    batchedDataset[batch][column.Key] = [.. tempBatches[batch][column.Key]];
            }

            for (int batch = 0; batch < batchCount; batch++) {
                float[] gradients = new float[weights["output"].Length];
                int batchLength = batchedDataset[batch].Values.First().Length;

                var singleton = new OutputFunctions.Singleton(rules, function, 1, weights);
                float[] wj = new float[rules.Values.First().Length];
                (int, float) r = singleton.GetR();

                Parallel.For(0, batchLength, row => {
                    float lowerMij = 0;

                    for (int i = 0; i < wj.Length; i++) {
                        Dictionary<string, float> input = [];
                        foreach(KeyValuePair<string, float[]> column in batchedDataset[batch])
                            input[column.Key == outputName ? "output" : column.Key] = column.Value[row];

                        wj[i] = singleton.GetDegree(input, i);
                        if (wj[i] > lowerMij)
                            lowerMij = wj[i];
                    }
                    float upperMij = (wj[r.Item1] >= r.Item2) ? r.Item2 : wj[r.Item1];

                    float result = upperMij / lowerMij;
                    /*if (isLast) {
                        realOut.Add(result);
                        expectedOut.Add(dataset[outputName][row]);
                    }*/
                    float deviation = new StandardDeviation().DerivativeCalculateRow(result, batchedDataset[batch][outputName][row]);
                    error += new StandardDeviation().CalculateRow(result, batchedDataset[batch][outputName][row]);

                    for (int i = 0; i < wj.Length; i++)
                        gradients[i] += learningRate * deviation * (wj[i] / lowerMij);
                });
                for (int i = 0; i < gradients.Length; i++)
                    weights["output"][i] -= gradients[i] / batchLength;
            }
            Console.WriteLine($"Error: {MathF.Sqrt(error / dataset.Values.First().Length)}. Error^2: {error / dataset.Values.First().Length}. Epochs: {epoch}");
            //if (isLast)
            //    Console.WriteLine($"Overall accuracy: {new DeterminationCoefficient().Calculate(realOut, expectedOut)}.");

            if ((epoch + 1) % autosaveCount == 0 && epoch > 0) {
                using StreamWriter outputFile = new(Path.Combine("/app/Dataset", "Weights.txt"));
                await outputFile.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(weights));
            }

            if ((epoch + 1) % 5 == 0)
                learningRate *= attenuationCoef;
        }

        return weights;
    }
}