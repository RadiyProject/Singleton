namespace Singleton.Models.MembershipFunctions;

public class Parabola(float leftBorder, float rightBorder, int areasCount) : MembershipFunction(leftBorder, rightBorder, areasCount, 3)
{
    protected override float FunctionRule(float value, float[] borders)
    {
        CheckBorders(borders, "параболы");
        return true switch
        {
            true when value >= borders[0] && value <= borders[1] => 1 - MathF.Pow((value - borders[0]) / (borders[1] - borders[0]), 2),
            true when value >= borders[1] && value <= borders[2] => 1 - MathF.Pow((borders[2] - value) / (borders[2] - borders[1]), 2),
            _ => 1
        };
    }

    protected override float DerivativeFunctionRule(float value, float[] borders)
    {
        CheckBorders(borders, "параболы");

        return true switch
        {
            true when value >= borders[0] && value <= borders[1] => -2f * (value - borders[0]) / MathF.Pow(borders[1] - borders[0], 2),
            true when value >= borders[1] && value <= borders[2] => 2f * (borders[1] - value) / MathF.Pow(borders[2] - borders[1], 2),
            _ => 0
        };
    }
}
