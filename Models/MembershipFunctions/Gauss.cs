namespace Singleton.Models.MembershipFunctions;

public class Gauss(float leftBorder, float rightBorder, int areasCount) : MembershipFunction(leftBorder, rightBorder, areasCount)
{
    public override float CalculateMembershipValue(float element, int areaId)
    {
        throw new NotImplementedException();
    }

    protected override float[,] DivideIntoMultipleAreas(int areasCount)
    {
        throw new NotImplementedException();
    }

    protected override float FunctionRule(float value, float[] borders)
    {
        throw new NotImplementedException();
    }
}
