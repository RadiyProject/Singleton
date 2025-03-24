using Microsoft.AspNetCore.Mvc;
using Singleton.Models;
using Singleton.Models.MembershipFunctions;

namespace Singleton.Controllers;
[Route("[controller]")]
[ApiController]
public class DatasetController : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> GetFile()
    {
        var dataset = await Dataset.ReadDataset("/app/Dataset/diamonds.csv", ["cut", "color", "clarity", "depth", "table"]/*, rowsCount: 5000*/);
        var result = Dataset.ConvertDatasetToNormalizedData(dataset);

        using (StreamWriter outputFile = new (Path.Combine("/app/Dataset", "PreparedDataset.txt")))
        {
            await outputFile.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }

        return Ok();
    }

    [HttpGet("{functionName}/{areasCount}")]
    public IActionResult GetFunction(string functionName, int areasCount)
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
        
        int points = areasCount * 200;
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

    [HttpGet("calculate/{functionName}/{areasCount}/{outputName}")]
    public async Task<IActionResult> Calculate(string functionName, int areasCount, string outputName)
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

        var normalized = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, float[]>>(
            await new StreamReader(Path.Combine("/app/Dataset", "PreparedDataset.txt")).ReadToEndAsync()) 
                ?? throw new InvalidDataException();

        float[] output = new float[normalized[outputName].Length];
        normalized[outputName].CopyTo(output, 0);
        float[] outputDistinct = [.. output.Distinct()];
        normalized.Remove(outputName);

        var result = FuzzyData.NormalizedDataToFuzzyData(normalized, function);

        var rules = FuzzyData.GenerateRules(result, outputDistinct);

        using (StreamWriter outputFile = new (Path.Combine("/app/Dataset", "RulesBase.txt")))
        {
            await outputFile.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(rules));
        }

        return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(rules));
    }

    [HttpPost("result")]
    public async Task<IActionResult> GetResult([FromBody] Dictionary<string, float> input)
    {
        MembershipFunction function = new Gauss(0, 1, 5);

        var rules = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, float[]>>(
            await new StreamReader(Path.Combine("/app/Dataset", "RulesBase.txt")).ReadToEndAsync()) 
                ?? throw new InvalidDataException();

        var result = new Models.OutputFunctions.Singleton(rules, function).CalculateOutput(input);

        return Ok(result);
    }
}