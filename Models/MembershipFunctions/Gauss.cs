namespace Singleton.Models.MembershipFunctions;

public class Gauss(float leftBorder, float rightBorder, int areasCount) : MembershipFunction(leftBorder, rightBorder, areasCount)
{
    protected override float FunctionRule(float value, float[] borders)
    {
        CheckBorders(borders, "функции Гаусса");

        float center = (borders[1] - borders[0]) * 0.5f;
        float sigma = (borders[1] - center) * 0.333334f;
        return (float)Math.Exp(-(value - center) * (value - center) * 0.5f / (sigma * sigma));
    }
}
