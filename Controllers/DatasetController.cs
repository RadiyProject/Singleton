using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Singleton.Models;

namespace Singleton.Controllers;
[Route("dataset")]
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

}