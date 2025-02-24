using OxyPlot;
using OxyPlot.Series;

public class PlotHelper
{
    public static PlotModel CreatePlot(FuzzySet set, double minX, double maxX, int steps)
    {
        var plotModel = new PlotModel { Title = $"Функция принадлежности: {set.Name}" };
        var series = new LineSeries();

        double stepSize = (maxX - minX) / steps;

        for (double x = minX; x <= maxX; x += stepSize)
        {
            series.Points.Add(new DataPoint(x, set.GetMembership(x)));
        }

        plotModel.Series.Add(series);
        return plotModel;
    }
}
