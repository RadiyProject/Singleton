namespace Singleton.Models.MembershipFunctions;

public abstract class MembershipFunction
{
    protected readonly float leftBorder;
    protected readonly float rightBorder;

    protected readonly float[,] areas; 
    protected int pointsCount;
    protected float initialOffset;

    public MembershipFunction(float leftBorder = 0, float rightBorder = 1, int areasCount = 3, int pointsCount = 2) 
    {
        if (leftBorder > rightBorder)
            (rightBorder, leftBorder) = (leftBorder, rightBorder);

        this.leftBorder = leftBorder;
        this.rightBorder = rightBorder;
        this.pointsCount = pointsCount;

        areas = DivideIntoMultipleAreas(areasCount > 0 ? areasCount : 3);
        //throw new Exception(Newtonsoft.Json.JsonConvert.SerializeObject(areas));
        initialOffset = (areas[0, areas.GetLength(1) - 1] - areas[0, 0]) * 0.5f;
    }

    public float CalculateMembershipValue(float element, int areaId)
    {
        float[] borders = new float[areas.GetLength(1)];

        for (int i = 0; i < borders.Length; i++)
            borders[i] = areas[areaId, i];
        
        return FunctionRule(element + initialOffset, borders);
    }

    protected float[,] DivideIntoMultipleAreas(int areasCount)
    {
        float[,] result = new float[areasCount, pointsCount];
        float step = (rightBorder - leftBorder) / pointsCount / (areasCount - (areasCount > 1 ? 1 : 0));
        for (int i = 0; i < areasCount; i++) {
            if (i == 0)
                result[i, 0] = 0;
            else
                result[i, 0] = result[i - 1, pointsCount - 1] - step;

            for (int j = 1; j < result.GetLength(1); j++)
                result[i, j] = result[i, j - 1] + step;
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
