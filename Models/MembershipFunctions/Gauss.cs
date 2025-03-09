namespace Singleton.Models.MembershipFunctions;

public class Gauss(float leftBorder, float rightBorder) : MembershipFunction(leftBorder, rightBorder)
{
    public override float CalculateMembershipValue(float element)
    {
        throw new NotImplementedException();
    }

    protected override float[,] DivideIntoMultipleAreas(int areasCount)
    {
        throw new NotImplementedException();
    }
}
