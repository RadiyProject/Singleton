using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;

public class DataLoader
{
    public static List<double[]> LoadData(string filePath)
    {
        var data = new List<double[]>();

        using (var workbook = new XLWorkbook(filePath))
        {
            var worksheet = workbook.Worksheet(1);

            for (int row = 2; row <= 21; row++)
            {
                var values = new double[3];

                for (int col = 1; col <= 3; col++)
                {
                    values[col - 1] = worksheet.Cell(row, col).GetDouble();
                }

                data.Add(values);
            }
        }

        return data;
    }
}
