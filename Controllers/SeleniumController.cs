using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Singleton.Controllers;
[Route("[controller]")]
[ApiController]
public class SeleniumController : ControllerBase
{
    [HttpGet("chrome")]
    public IActionResult CheckChrome()
    {
        var driverOptions = new ChromeOptions();
        using var driver = new RemoteWebDriver(new Uri("http://selenium-hub:4444"), driverOptions);

        driver.Url = "https://musescore.com";
        driver.FindElement(By.Name("q")).SendKeys("webdriver" + Keys.Return);
        Console.WriteLine(driver.Title);

        return Ok();
    }
}
