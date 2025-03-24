using Singleton.Models.MembershipFunctions;

namespace Singleton.Models.OutputFunctions;

public abstract class OutputFunction(Dictionary<string, float[]> rules, MembershipFunction function)
{
    protected readonly Dictionary<string, float[]> rules = rules;
    protected readonly MembershipFunction function = function;

    public abstract float CalculateOutput(Dictionary<string, float> input);
}
