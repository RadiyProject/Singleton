namespace Singleton.Models.MembershipFunctions;

public abstract class MembershipFunction
{
    private readonly float leftBorder;
    private readonly float rightBorder;

    private readonly float[,] areas; 

    public MembershipFunction(float leftBorder = 0, float rightBorder = 1, int areasCount = 3) 
    {
        if (leftBorder > rightBorder)
            (rightBorder, leftBorder) = (leftBorder, rightBorder);

        this.leftBorder = leftBorder;
        this.rightBorder = rightBorder;

        areas = DivideIntoMultipleAreas(areasCount);
    }

    public abstract float CalculateMembershipValue(float element);

    protected abstract float[,] DivideIntoMultipleAreas(int areasCount);
}
