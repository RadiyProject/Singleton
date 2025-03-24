namespace Singleton.Models.OutputFunctions;

public abstract class OutputFunction(Dictionary<string, float[]> rules)
{
    protected readonly Dictionary<string, float[]> rules = rules;

    public abstract float CalculateOutput(Dictionary<string, float> input);
}
