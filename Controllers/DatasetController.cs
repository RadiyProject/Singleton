using Microsoft.AspNetCore.Mvc;
using Singleton.Models;
using Singleton.Models.MachineLearning;
using Singleton.Models.MachineLearning.Deviations;
using Singleton.Models.MembershipFunctions;

namespace Singleton.Controllers;
[Route("[controller]")]
[ApiController]
public class DatasetController : ControllerBase
{
    private readonly MembershipFunction function = new Triangle(0, 1, 7);

    [HttpGet("")]
    public async Task<IActionResult> GetFile()
    {
        var dataset = await Dataset.ReadDataset("/app/Dataset/diamonds.csv", ["carat", "price", "x (Premium)", "z (Very Good)"]/*, rowsCount: 5000*/);
        var result = Dataset.ConvertDatasetToNormalizedData(dataset);

        using (StreamWriter outputFile = new (Path.Combine("/app/Dataset", "PreparedDataset.txt")))
        {
            await outputFile.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }

        return Ok();
    }

    [HttpGet("distinct")]
    public async Task<IActionResult> GetDistinctOutputs()
    {
        var dataset = await Dataset.ReadDataset("/app/Dataset/diamonds.csv", ["cut"]/*, rowsCount: 5000*/);
        var distincts = dataset.Values.ToArray();
        List<string>[] distinctOutput = new List<string>[distincts.Length];
        int i = 0;
        foreach(List<string> distinct in distincts) 
        {
            distinctOutput[i] = [.. distinct.Distinct()];
            i++;
        }
        
        var result = Dataset.ConvertOutputToCategories(distinctOutput[0]);

        using (StreamWriter outputFile = new (Path.Combine("/app/Dataset", "PreparedDistinct.txt")))
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

    [HttpGet("calculate/{outputName}")]
    public async Task<IActionResult> Calculate(string outputName)
    {
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

        return Ok("Ok");
    }

    [HttpPost("result")]
    public async Task<IActionResult> GetResult([FromBody] Dictionary<string, float> input)
    {
        var rules = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, float[]>>(
            await new StreamReader(Path.Combine("/app/Dataset", "RulesBase.txt")).ReadToEndAsync()) 
                ?? throw new InvalidDataException();

        var distinct = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, float[]>>(
            await new StreamReader(Path.Combine("/app/Dataset", "PreparedDistinct.txt")).ReadToEndAsync()) 
                ?? throw new InvalidDataException();

        var result = new Models.OutputFunctions.Singleton(rules, function, 1).CalculateOutput(input);
        var category = Models.OutputFunctions.Singleton.DefuzzToCategory(result, distinct);

        return Ok(category);
    }

    [HttpGet("separate")]
    public async Task<IActionResult> SeparateDataset([FromQuery] float trainProportion)
    {
        var normalized = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, float[]>>(
            await new StreamReader(Path.Combine("/app/Dataset", "PreparedDataset.txt")).ReadToEndAsync()) 
                ?? throw new InvalidDataException();

        var separated = Dataset.Separate(normalized, trainProportion);

        using (StreamWriter outputFile = new (Path.Combine("/app/Dataset", "Train.txt")))
        {
            await outputFile.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(separated[0]));
        }

        using (StreamWriter outputFile = new (Path.Combine("/app/Dataset", "Test.txt")))
        {
            await outputFile.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(separated[1]));
        }

        return Ok("Ok");
    }

    [HttpGet("reduce")]
    public async Task<IActionResult> ReduceDataset([FromQuery] float reduceValue, [FromQuery] string fileName="Train")
    {
        var dataset = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, float[]>>(
            await new StreamReader(Path.Combine("/app/Dataset", $"{fileName}.txt")).ReadToEndAsync()) 
                ?? throw new InvalidDataException();

        var reduced = Dataset.Reduce(dataset, reduceValue);
        using (StreamWriter outputFile = new (Path.Combine("/app/Dataset", $"{fileName}Reduced.txt")))
        {
            await outputFile.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(reduced));
        }

        return Ok("Ok");
    }

    [HttpGet("weights")]
    public async Task<IActionResult> GenerateWeights()
    {
        var rules = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, float[]>>(
            await new StreamReader(Path.Combine("/app/Dataset", "RulesBase.txt")).ReadToEndAsync()) 
                ?? throw new InvalidDataException();

        var weights = Weights.Generate(rules);
        using (StreamWriter outputFile = new (Path.Combine("/app/Dataset", "Weights.txt")))
        {
            await outputFile.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(weights));
        }

        return Ok("Ok");
    }

    [HttpPost("weights/result")]
    public async Task<IActionResult> GetResultWithWeights([FromBody] Dictionary<string, float> input)
    {
        var rules = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, float[]>>(
            await new StreamReader(Path.Combine("/app/Dataset", "RulesBase.txt")).ReadToEndAsync()) 
                ?? throw new InvalidDataException();

        var distinct = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, float[]>>(
            await new StreamReader(Path.Combine("/app/Dataset", "PreparedDistinct.txt")).ReadToEndAsync()) 
                ?? throw new InvalidDataException();

        var weights = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, float[]>>(
            await new StreamReader(Path.Combine("/app/Dataset", "Weights.txt")).ReadToEndAsync()) 
                ?? throw new InvalidDataException();

        var result = new Models.OutputFunctions.Singleton(rules, function, 1, weights).CalculateOutput(input);
        //var category = Models.OutputFunctions.Singleton.DefuzzToCategory(result, distinct);

        return Ok(result);
    }

    [HttpPost("train")]
    public async Task<IActionResult> Train([FromQuery] int epochsCount = 5)
    {
        var rules = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, float[]>>(
            await new StreamReader(Path.Combine("/app/Dataset", "RulesBase.txt")).ReadToEndAsync()) 
                ?? throw new InvalidDataException();

        var weights = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, float[]>>(
            await new StreamReader(Path.Combine("/app/Dataset", "Weights.txt")).ReadToEndAsync()) 
                ?? throw new InvalidDataException();

        var dataset = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, float[]>>(
            await new StreamReader(Path.Combine("/app/Dataset", "TrainReduced.txt")).ReadToEndAsync()) 
                ?? throw new InvalidDataException();

        weights = await new GradientDescent().Train(rules, weights, dataset, function, "carat", epochsCount);

        using (StreamWriter outputFile = new (Path.Combine("/app/Dataset", "Weights.txt")))
        {
            await outputFile.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(weights));
        }

        return Ok("Ok");
    }

    [HttpGet("check-test")]
    public async Task<IActionResult> CheckTest()
    {
        var rules = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, float[]>>(
            await new StreamReader(Path.Combine("/app/Dataset", "RulesBase.txt")).ReadToEndAsync()) 
                ?? throw new InvalidDataException();

        var weights = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, float[]>>(
            await new StreamReader(Path.Combine("/app/Dataset", "Weights.txt")).ReadToEndAsync()) 
                ?? throw new InvalidDataException();

        var dataset = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, float[]>>(
            await new StreamReader(Path.Combine("/app/Dataset", "TestReduced.txt")).ReadToEndAsync()) 
                ?? throw new InvalidDataException();

        string output = "carat";
        float error = 0;
        float predicted = 0;
        float expected = 0;
        for(int i = 0; i < dataset.First().Value.Length; i++) {
            Dictionary<string, float> input = [];
            foreach(KeyValuePair<string, float[]> column in dataset)
                input[column.Key == output ? "output" : column.Key] = column.Value[i];

            float result = new Models.OutputFunctions.Singleton(rules, function, 1, weights).CalculateOutput(input);
            if (MathF.Abs(predicted - expected) > MathF.Abs(result - dataset[output][i]) || predicted == 0 && expected == 0) {
                predicted = result;
                expected = dataset[output][i];
            }
            error += new StandardDeviation().CalculateRow(result, dataset[output][i]);
        }

        return Ok("Error: " + error / dataset.First().Value.Length + ". Min predicted: " + predicted + ". Min expected: " + expected);
    }
}