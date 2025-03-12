namespace Singleton.Models.MembershipFunctions;

public class Trapezoid(float leftBorder, float rightBorder, int areasCount) : MembershipFunction(leftBorder, rightBorder, areasCount, 4)
{
    protected override float FunctionRule(float value, float[] borders)
    {
        CheckBorders(borders, "трапеции");
        return true switch
        {
            true when value >= borders[0] && value <= borders[1] => (value - borders[0]) / (borders[1] - borders[0]),
            true when value >= borders[1] && value <= borders[2] => 1,
            true when value >= borders[2] && value <= borders[3] => (borders[3] - value) / (borders[3] - borders[2]),
            _ => 0
        };
    }
}
