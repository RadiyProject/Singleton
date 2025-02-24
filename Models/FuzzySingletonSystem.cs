using System;
using System.Collections.Generic;

public class FuzzySingletonSystem
{
    public List<FuzzySet> InputSets { get; set; } = new List<FuzzySet>();
    public List<double> SingletonValues { get; set; } = new List<double>();

    public FuzzySingletonSystem(List<FuzzySet> inputSets, List<double> singletonValues)
    {
        InputSets = inputSets;
        SingletonValues = singletonValues;
    }

    public double ComputeOutput(double[] inputs)
    {
        double numerator = 0, denominator = 0;

        for (int i = 0; i < SingletonValues.Count; i++)
        {
            double weight = 1;

            for (int j = 0; j < inputs.Length; j++)
            {
                weight *= InputSets[i].GetMembership(inputs[j]);
            }

            numerator += weight * SingletonValues[i];
            denominator += weight;
        }

        return denominator == 0 ? 0 : numerator / denominator;
    }
}
