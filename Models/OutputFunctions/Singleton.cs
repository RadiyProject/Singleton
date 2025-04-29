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
        
        float m = 0;
        float rm = 0;
        for(int i = 0; i < ruleLength; i++) {
            float mj = Multiply(rules, input, i);
            m += mj;
            rm += rules["output"][i] * weights!["output"][i] * mj;
        }
        
        if(m == 0)
            m = 0.001f;

        return (float)(rm / m);
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

            max += function.CalculateDerivativeMembershipValue(input[rule.Key] + (weights != null ? weights[rule.Key][idx] : 0), (int)rule.Value[idx]);
        }

        return max;
    }

    private float Multiply(Dictionary<string, float[]> rules, 
        Dictionary<string, float> input, int idx)
    {
        float? result = null;
        foreach(KeyValuePair<string, float[]> rule in rules) {
            if (rule.Key == "output")
                continue;

            float value = function.CalculateMembershipValue(input[rule.Key], (int)rule.Value[idx]);

            result = result == null ? value : result * value;
        }
        if(result == null)
            throw new Exception("Result incorrect");

        return (float)result;
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