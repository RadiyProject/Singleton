namespace Singleton.Models.MembershipFunctions;

public abstract class MembershipFunction
{
    protected readonly float leftBorder;
    protected readonly float rightBorder;

    public readonly float[,] areas; 
    protected int pointsCount;
    protected float initialOffset;

    public int AreasCount { get; }

    public MembershipFunction(float leftBorder = 0, float rightBorder = 1, int areasCount = 3, int pointsCount = 2) 
    {
        if (areasCount <= 0 || pointsCount < 2)
            throw new InvalidDataException();

        if (leftBorder > rightBorder)
            (rightBorder, leftBorder) = (leftBorder, rightBorder);

        this.leftBorder = leftBorder;
        this.rightBorder = rightBorder;
        this.pointsCount = pointsCount;

        areas = DivideIntoMultipleAreas(areasCount);
        AreasCount = areasCount;
    }

    public float CalculateMembershipValue(float element, int areaId)
    {
        float[] borders = new float[areas.GetLength(1)];

        for (int i = 0; i < borders.Length; i++)
            borders[i] = areas[areaId, i];
        
        return FunctionRule(element, borders);
    }

    protected float[,] DivideIntoMultipleAreas(int areasCount)
    {
        float[,] result = new float[areasCount, pointsCount];
        float step = (rightBorder - leftBorder) / (areasCount - (areasCount > 1 ? 1 : 0));
        float pointStep = step / (pointsCount - 1);
        for (int i = 0; i < areasCount; i++) {
            if (i == 0)
                result[i, 0] = areasCount > 1 ? -step : 0;
            else
                result[i, 0] = result[i - 1, 0] + step;

            for (int j = 1; j < result.GetLength(1); j++)
                result[i, j] = result[i, j - 1] + pointStep * (areasCount > 1 ? 2 : 1);
        }

        return result;
    }

    protected abstract float FunctionRule(float value, float[] borders); 

    protected void CheckBorders(float[] borders, string functionName = "функции") 
    {
        string exceptionMessage = $"Точки границ {functionName} заданы некорретно";
        if (borders.Length != pointsCount)
            throw new InvalidDataException(exceptionMessage);
        
        for (int i = 1; i < borders.Length; i++)
            if (borders[i - 1] > borders[i])
                throw new InvalidDataException(exceptionMessage);
    }
}
