namespace Singleton.Models.MachineLearning.Deviations;

public class AverageAbsoluteDeviation() : Deviation
{
    public override float Calculate(List<float> realOut, List<float> expectedOut)
    {
        if (realOut.Count != expectedOut.Count)
            throw new InvalidDataException(invalidDataExceptionMessage);

        float sum = 0;
        foreach(var(real, expected) in realOut.Zip(expectedOut, (x, y) => (real: x, expected: y)))
            sum += Math.Abs(real - expected);

        return sum / realOut.Count;
    }
}
