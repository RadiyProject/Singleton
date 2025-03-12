namespace Singleton.Models.MembershipFunctions;

public class Parabola(float leftBorder, float rightBorder, int areasCount) : MembershipFunction(leftBorder, rightBorder, areasCount)
{
    protected override float FunctionRule(float value, float[] borders)
    {
        CheckBorders(borders, "параболы");
        return true switch
        {
            true when value >= (borders[0] - borders[1]) && value <= (borders[0] + borders[1]) 
                => 1 - (value - borders[0]) * (value - borders[0]) / borders[1] * borders[1],
            _ => 0
        };
    }
}
