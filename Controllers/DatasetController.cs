using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Singleton.Models;
using Singleton.Models.MembershipFunctions;

namespace Singleton.Controllers;
[Route("[controller]")]
[ApiController]
public class DatasetController : ControllerBase
{
    [HttpGet("")]
    public IActionResult GetFile()
    {
        var dataset = Dataset.ReadDataset("/app/Dataset/diamonds.csv", ["cut", "color", "clarity", "depth", "table"], rowsCount: 5000);
        var result = Dataset.ConvertDatasetToNormalizedData(dataset);

        return Ok("{" + string.Join("\n", result.Select(kv => kv.Key + "=" + JsonSerializer.Serialize(kv.Value)).ToArray()) + "}");
    }

    [HttpGet("{functionName}/{areasCount}")]
    public IActionResult GetTriangle(string functionName, int areasCount)
    {

        MembershipFunction function;
        if (functionName == "trapezoid")
            function = new Trapezoid(0, 1, areasCount);
        else if (functionName == "parabola")
            function = new Parabola(0, 1, areasCount);
        else if (functionName == "gauss")
            function = new Gauss(0, 1, areasCount);
        else
            function = new Triangle(0, 1, areasCount);
        
        int points = 100;
        float step = 1f / points;
        List<Dictionary<string, float>> result = [];
        for (int j = 0; j < areasCount; j++) {
            float currentX = 0;
            for (int i = 0; i < points; i++)
            {
                Dictionary<string, float> temp = [];
                temp["x"] = currentX;
                temp["y"] = function.CalculateMembershipValue(currentX, j);
                result.Add(temp);

                currentX += step;
            }
        }
        Dictionary<string, object> resultResult = [];
        resultResult["Items"] = result;
        resultResult["Count"] = result.Count;

        return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(resultResult));
    }
}