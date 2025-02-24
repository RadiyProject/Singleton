using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Singleton.Models;

namespace Singleton.Controllers
{
    [Route("dataset")]
    [ApiController]
    public class DatasetController : ControllerBase
    {
        [HttpGet("")]
        public IActionResult GetFile()
        {
            return Ok(JsonSerializer.Serialize(Dataset.ReadDataset("/app/Dataset/diamonds.csv", ["cut", "color", "clarity", "depth", "table"])));
        }
    }
}
