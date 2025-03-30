namespace Singleton.Models.MachineLearning.Deviations;

public abstract class Deviation
{
    protected string invalidDataExceptionMessage = "Количество ожидаемых и фактических выходов различается";

    public abstract float Calculate(List<float> realOut, List<float> expectedOut);
}
