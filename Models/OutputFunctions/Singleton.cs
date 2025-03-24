using Singleton.Models.MembershipFunctions;

namespace Singleton.Models.OutputFunctions;

public class Singleton(Dictionary<string, float[]> rules, MembershipFunction function) : 
    OutputFunction(rules, function)
{
    protected float MaxValue = 1;

    public override float CalculateOutput(Dictionary<string, float> input)
    {
        int ruleLength = rules["output"].Length;
        if (ruleLength == 0)
            throw new InvalidDataException("База правил пуста");

        float r = rules["output"][0];
        int rIdx = 0;
        for(int i = 1; i < ruleLength; i++)
            if (rules["output"][i] > r) {
                r = rules["output"][i];
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

    private float Multiply(Dictionary<string, float[]> rules, 
        Dictionary<string, float> input, int idx)
    {
        float min = MaxValue;
        foreach(KeyValuePair<string, float[]> rule in rules) {
            if (rule.Key == "output")
                continue;

            float value = function.CalculateMembershipValue(input[rule.Key], (int)rule.Value[idx]);
            if (value <= 0.01f) 
                value = MaxValue;

            if (min > value)
                min = value;
        }

        return min;
    }
}