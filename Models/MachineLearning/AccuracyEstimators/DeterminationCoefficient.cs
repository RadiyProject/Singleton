namespace Singleton.Models.MachineLearning.AccuracyEstimators;

public class DeterminationCoefficient
{
    public float Calculate(List<float> realOut, List<float> expectedOut)
    {
        if (realOut.Count != expectedOut.Count)
            throw new InvalidDataException("Количество ожидаемых и фактических выходов различается");

        float meanExpected = 0;
        foreach(float expected in expectedOut)
            meanExpected += expected;
        meanExpected /= expectedOut.Count;

        float sumUpper = 0;
        float sumLower = 0;
        foreach(var(real, expected) in realOut.Zip(expectedOut, (x, y) => (real: x, expected: y))) {
            sumUpper += (real - expected) * (real - expected);
            sumLower += (meanExpected - expected) * (meanExpected - expected);
        }

        return 1 - sumUpper / sumLower;
    }
}
