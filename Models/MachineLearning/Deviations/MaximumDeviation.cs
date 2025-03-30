namespace Singleton.Models.MachineLearning.Deviations;

public class MaximumDeviation : Deviation
{
    public override float Calculate(List<float> realOut, List<float> expectedOut)
    {
        if (realOut.Count != expectedOut.Count)
            throw new InvalidDataException(invalidDataExceptionMessage);

        float? max = null;
        foreach(var(real, expected) in realOut.Zip(expectedOut, (x, y) => (real: x, expected: y))) {
            float current = Math.Abs(real - expected);
            if (current > max || max == null)
                max = current;
        }

        return max ?? 0;
    }
}
