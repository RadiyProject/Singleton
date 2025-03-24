namespace Singleton.Models.OutputFunctions;

public class Singleton(Dictionary<string, float[]> rules) : OutputFunction(rules)
{
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
        
        float mij = Multiply(rules, rIdx);
        float upperMij = mij >= r ? r : mij;
        float downMij = upperMij;
        for(int i = 0; i < ruleLength; i++) {
            mij = Multiply(rules, i);
            if (mij > downMij)
                downMij = mij;
        }
        
        return upperMij / downMij;
    }

    private static float Multiply(Dictionary<string, float[]> rules, int rIdx)
    {
        float min = rules.Keys.First()[rIdx];
        foreach(KeyValuePair<string, float[]> rule in rules)
            if(min > rule.Value[rIdx])
                min = rule.Value[rIdx];

        return min;
    }
}