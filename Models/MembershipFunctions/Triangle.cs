namespace Singleton.Models.MembershipFunctions;

public class Triangle(float leftBorder, float rightBorder, int areasCount) : MembershipFunction(leftBorder, rightBorder, areasCount, 3)
{
    protected override float FunctionRule(float value, float[] borders)
    {
        CheckBorders(borders, "треугольника");
        return true switch
        {
            true when value >= borders[0] && value <= borders[1] => (value - borders[0]) / (borders[1] - borders[0]),
            true when value >= borders[1] && value <= borders[2] => (borders[2] - value) / (borders[2] - borders[1]),
            _ => 0
        };
    }

    protected override float DerivativeFunctionRule(float value, float[] borders)
    {
        CheckBorders(borders, "треугольника");

        return true switch
        {
            true when value >= borders[0] && value <= borders[1] => 1f / (borders[1] - borders[0]),
            true when value >= borders[1] && value <= borders[2] => -1f / (borders[2] - borders[1]),
            _ => 0
        };
    }
}