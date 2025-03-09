namespace Singleton.Models.MembershipFunctions;

public class Triangle(float leftBorder, float rightBorder, int areasCount) : MembershipFunction(leftBorder, rightBorder, areasCount)
{
    public override float CalculateMembershipValue(float element, int areaId)
    {
        //TODO: посчитать границы на основе конкретной области и получить результат принадлежности
        throw new NotImplementedException();
    }

    protected override float[,] DivideIntoMultipleAreas(int areasCount)
    {
        float[,] result = new float[areasCount, 3];
        float step = (rightBorder - leftBorder) / areasCount;
        for (int i = 0; i < areasCount; i++)
            if (i == 0) {
                result[i, 0] = 0;
                result[i, 1] = 0;
                result[i, 2] = step;
            }
            else if (i == areasCount - 1)
            {
                result[i, 0] = result[i - 1, 1];
                result[i, 1] = result[i, 0] + step;
                result[i, 2] = result[i, 1];
            }
            else
            {
                result[i, 0] = result[i - 1, 1];
                result[i, 1] = result[i - 1, 2];
                result[i, 2] = result[i, 1] + step;
            }

        return result;
    }

    protected override float FunctionRule(float value, float[] borders)
    {
        if (borders.Length != 3 || borders[0] > borders[1] || borders[1] > borders[2])
            throw new InvalidDataException("Точки границ треугольника заданы некорретно");

        return true switch
        {
            true when value >= borders[0] && value <= borders[1] => (value - borders[0]) / (borders[1] - borders[0]),
            true when value >= borders[1] && value <= borders[2] => (borders[2] - value) / (borders[2] - borders[1]),
            _ => 0
        };
    }
}