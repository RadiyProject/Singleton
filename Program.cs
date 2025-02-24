using System;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.SkiaSharp;
using OxyPlot.Series;

class Program
{
    static void Main()
    {
        string filePath = "Data/input.xlsx";

        if (!System.IO.File.Exists(filePath))
        {
            Console.WriteLine("Файл input.xlsx не найден!");
            return;
        }

        List<double[]> data = DataLoader.LoadData(filePath);

        Console.Write("Количество термов: ");
        int termCount = int.Parse(Console.ReadLine());

        Console.WriteLine("Функция принадлежности:");
        Console.WriteLine("1 - Треугольная");
        Console.WriteLine("2 - Трапециевидная");
        Console.WriteLine("3 - Параболическая");
        Console.WriteLine("4 - Гауссовская");

        int choice = int.Parse(Console.ReadLine());
        MembershipFunctionType selectedType = choice switch
        {
            1 => MembershipFunctionType.Triangular,
            2 => MembershipFunctionType.Trapezoidal,
            3 => MembershipFunctionType.Parabolic,
            4 => MembershipFunctionType.Gaussian,
            _ => throw new ArgumentException("Неверный выбор")
        };

        List<FuzzySet> fuzzySets = new List<FuzzySet>();
        List<double> singletonValues = new List<double>();

        double minX = 0, maxX = 10;  
        double step = (maxX - minX) / (termCount - 1);

        for (int i = 0; i < termCount; i++)
        {
            double center = minX + i * step;
            FuzzySet set = selectedType switch
            {
                MembershipFunctionType.Triangular => new FuzzySet($"Term {i+1}", selectedType, new double[] { center - 1, center, center + 1 }),
                MembershipFunctionType.Trapezoidal => new FuzzySet($"Term {i+1}", selectedType, new double[] { center - 2, center - 1, center + 1, center + 2 }),
                MembershipFunctionType.Parabolic => new FuzzySet($"Term {i+1}", selectedType, new double[] { center - 2, center + 2 }),
                MembershipFunctionType.Gaussian => new FuzzySet($"Term {i+1}", selectedType, new double[] { center, 1 }),
                _ => throw new NotImplementedException()
            };

            fuzzySets.Add(set);
            singletonValues.Add(center);  
        }

        var fuzzySystem = new FuzzySingletonSystem(fuzzySets, singletonValues);

        Console.WriteLine("Выходные значения:");
        foreach (var row in data)
        {
            double output = fuzzySystem.ComputeOutput(row);
            Console.WriteLine($"Вход: {string.Join(", ", row)} → Выход: {output:F3}");
        }

        var plotModel = PlotHelper.CreatePlot(fuzzySets[0], minX, maxX, 100);

        using (var stream = new System.IO.FileStream("plot.png", System.IO.FileMode.Create))
        {
            var pngExporter = new OxyPlot.SkiaSharp.PngExporter { Width = 600, Height = 400 };
            pngExporter.Export(plotModel, stream);
        }
    }
}
