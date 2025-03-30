using Singleton.Models.MembershipFunctions;

namespace Singleton.Models.OutputFunctions;

public abstract class OutputFunction(Dictionary<string, float[]> rules, MembershipFunction function, float upperBorder, 
    Dictionary<string, float[]>? weights)
{
    protected readonly Dictionary<string, float[]> rules = rules;
    protected readonly Dictionary<string, float[]>? weights = weights;
    protected readonly MembershipFunction function = function;
    protected readonly float upperBorder = upperBorder;

    public abstract float CalculateOutput(Dictionary<string, float> input);
}
