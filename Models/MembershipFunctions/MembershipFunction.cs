namespace Singleton.Models.MembershipFunctions;

public abstract class MembershipFunction
{
    protected readonly float leftBorder;
    protected readonly float rightBorder;

    protected readonly float[,] areas; 

    public MembershipFunction(float leftBorder = 0, float rightBorder = 1, int areasCount = 3) 
    {
        if (leftBorder > rightBorder)
            (rightBorder, leftBorder) = (leftBorder, rightBorder);

        this.leftBorder = leftBorder;
        this.rightBorder = rightBorder;

        areas = DivideIntoMultipleAreas(areasCount > 0 ? areasCount : 3);
    }

    public abstract float CalculateMembershipValue(float element, int areaId);

    protected abstract float[,] DivideIntoMultipleAreas(int areasCount);

    protected abstract float FunctionRule(float value, float[] borders); 
}
