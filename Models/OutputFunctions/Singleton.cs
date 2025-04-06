using Singleton.Models.MembershipFunctions;

namespace Singleton.Models.OutputFunctions;

public class Singleton(Dictionary<string, float[]> rules, MembershipFunction function, float upperBorder, Dictionary<string, float[]>? weights = null) : 
    OutputFunction(rules, function, upperBorder, weights)
{
    public override float CalculateOutput(Dictionary<string, float> input)
    {
        int ruleLength = rules["output"].Length;
        if (ruleLength == 0)
            throw new InvalidDataException("База правил пуста");

        float r = rules["output"][0] * (weights != null ? weights["output"][0] : 1);
        int rIdx = 0;
        for(int i = 1; i < ruleLength; i++)
            if (rules["output"][i] * (weights != null ? weights["output"][i] : 1) > r) {
                r = rules["output"][i] * (weights != null ? weights["output"][i] : 1);
                rIdx = i;
            }
        
        float mij = Multiply(rules, input, rIdx);
        float upperMij = (mij >= r) ? r : mij;
        float downMij = upperMij;
        for(int i = 0; i < ruleLength; i++) {
            mij = Multiply(rules, input, i);
            if (mij > downMij)
                downMij = mij;
        }
        
        return upperMij / downMij;
    }

    public (int, float) GetR()
    {
        int ruleLength = rules["output"].Length;
        if (ruleLength == 0)
            throw new InvalidDataException("База правил пуста");

        float r = rules["output"][0] * (weights != null ? weights["output"][0] : 1);
        int rIdx = 0;
        for(int i = 1; i < ruleLength; i++)
            if (rules["output"][i] * (weights != null ? weights["output"][i] : 1) > r) {
                r = rules["output"][i] * (weights != null ? weights["output"][i] : 1);
                rIdx = i;
            }
        
        return (rIdx, r);
    }

    public float GetDegree(Dictionary<string, float> input, int rIdx)
    {   
        return Multiply(rules, input, rIdx);
    }

    public float GetDerivated(Dictionary<string, float> input, int rIdx)
    {   
        return Add(rules, input, rIdx, weights);
    }

    private float Add(Dictionary<string, float[]> rules, 
        Dictionary<string, float> input, int idx, Dictionary<string, float[]>? weights = null)
    {
        float max = 0;
        foreach(KeyValuePair<string, float[]> rule in rules) {
            if (rule.Key == "output")
                continue;

            float value = function.CalculateDerivativeMembershipValue(input[rule.Key] + (weights != null ? weights[rule.Key][idx] : 0), (int)rule.Value[idx]);

            if (max < value)
                max = value;
        }

        return max;
    }

    private float Multiply(Dictionary<string, float[]> rules, 
        Dictionary<string, float> input, int idx)
    {
        float min = upperBorder;
        foreach(KeyValuePair<string, float[]> rule in rules) {
            if (rule.Key == "output")
                continue;

            float value = function.CalculateMembershipValue(input[rule.Key], (int)rule.Value[idx]);
            if (value <= 0.01f) 
                value = upperBorder;

            if (min > value)
                min = value;
        }

        return min;
    }

    public static string DefuzzToCategory(float value, Dictionary<string, float[]> distinctOutputs)
    {
        float diff = MathF.Abs(value - distinctOutputs.Values.First()[0]);
        string name = distinctOutputs.Keys.First();
        foreach(KeyValuePair<string, float[]> distinctOutput in distinctOutputs)
        {
            float tempDiff = MathF.Abs(value - distinctOutput.Value[0]);
            if (diff > tempDiff)
            {
                diff = tempDiff;
                name = distinctOutput.Key;
            }
        }

        return name;
    }
}