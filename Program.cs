using System;
using System.IO;
using System.Linq;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.ImageSharp; // Подключаем ImageSharp для рендеринга графиков

class Program
{
    static void Main()
    {
        // Данные
        double[] rain = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0.6, 0.6, 0, 0, 0.2, 0, 0.1, 0, 0.1, 0.2 };
        double[] snowfall = { 0, 0, 0, 0, 0, 0, 0.1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        double[] cloud_cover = { 0.91, 1, 1, 1, 1, 0.99, 0.68, 0.14, 0.29, 0.59, 0.52, 0.9, 0.8, 0.35, 1, 0.97, 0.47, 0.94, 1, 0.98, 0.94, 0.59, 0.48, 0.55, 0.49 };
        double[] weather_code = { 0.03, 0.03, 0.03, 0.03, 0.03, 0.03, 0.02, 0, 0.01, 0.02, 0.02, 0.03, 0.03, 0.01, 0.03, 0.53, 0.53, 0.03, 0.03, 0.51, 0.03, 0.51, 0.01, 0.51, 0.51 };

        int numTerms = 3; // Количество термов
        PlotFuzzyMembership("Rainfall", rain, numTerms);
        PlotFuzzyMembership("Cloud Cover", cloud_cover, numTerms);
        PlotFuzzyMembership("Weather Code", weather_code, numTerms);
        PlotFuzzyMembership("Snowfall", snowfall, numTerms);
    }

    static void PlotFuzzyMembership(string title, double[] data, int numTerms)
    {
        double min = data.Min();
        double max = data.Max();
        double step = (max - min) / (numTerms - 1);

        var plotModel = new PlotModel { Title = $"{title} Membership Functions" };

        for (int i = 0; i < numTerms; i++)
        {
            double center = min + i * step;
            var series = new LineSeries { Title = $"Term {i - 1}" };

            for (double x = min; x <= max; x += (max - min) / 100)
            {
                double y = TriangularMembership(x, center, step);
                series.Points.Add(new DataPoint(x, y));
            }

            plotModel.Series.Add(series);
        }

        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = title });
        plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Membership" });

        SavePlotAsPng(plotModel, $"{title}.png");
    }

    static double TriangularMembership(double x, double center, double width)
    {
        double left = center - width;
        double right = center + width;

        if (x <= left || x >= right) return 0;
        if (x == center) return 1;

        return x < center ? (x - left) / (center - left) : (right - x) / (right - center);
    }

    static void SavePlotAsPng(PlotModel model, string fileName)
    {
        var exporter = new PngExporter(800, 600, 96); // 800x600 px, 96 dpi
        using (var stream = File.Create(fileName))  // Создаем поток файла
        {
            exporter.Export(model, stream); // Экспортируем график в PNG
        }
        Console.WriteLine($"График сохранен в {fileName}");
    }
}
