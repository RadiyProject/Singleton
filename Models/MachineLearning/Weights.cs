namespace Singleton.Models.MachineLearning;

public class Weights
{
    public static Dictionary<string, float[]> Generate(Dictionary<string, float[]> rulesBase)
    {
        Dictionary<string, float[]> weights = [];
        foreach(KeyValuePair<string, float[]> rule in rulesBase) {
            weights[rule.Key] = new float[rule.Value.Length];
            for(int i = 0; i < rule.Value.Length; i++)
            {
                if(rule.Key != "output")
                    weights[rule.Key][i] = new Random().NextSingle();
                else
                    weights[rule.Key][i] = 1f;
            }
        }

        return weights;
    }
}
