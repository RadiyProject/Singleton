namespace Singleton.Models.MembershipFunctions;

public class Triangle(float leftBorder, float rightBorder, int areasCount) : MembershipFunction(leftBorder, rightBorder, areasCount)
{
    protected new const int pointsCount = 3;
    protected new const float initialOffset = -1f;

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
}