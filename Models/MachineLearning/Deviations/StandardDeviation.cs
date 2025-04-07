namespace Singleton.Models.MachineLearning.Deviations;

public class StandardDeviation() : Deviation
{
    public override float Calculate(List<float> realOut, List<float> expectedOut)
    {
        if (realOut.Count != expectedOut.Count)
            throw new InvalidDataException(invalidDataExceptionMessage);

        float sum = 0;
        foreach(var(real, expected) in realOut.Zip(expectedOut, (x, y) => (real: x, expected: y)))
            sum += (real - expected) * (real - expected);

        return (float)Math.Sqrt(sum / realOut.Count);
    }

    public float CalculateRow(float realOut, float expectedOut)
    {
        return (realOut - expectedOut) * (realOut - expectedOut);
    }

    public float DerivativeCalculateRow(float realOut, float expectedOut)
    {
        return 2f * (-realOut + expectedOut);
    }
}
