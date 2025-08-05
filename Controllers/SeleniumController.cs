using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Singleton.Controllers;
[Route("[controller]")]
[ApiController]
public class SeleniumController : ControllerBase
{
    protected IWebDriver driver;

    public SeleniumController() {
        var driverOptions = new ChromeOptions();
        driverOptions.AddArgument("no-sandbox");
        driver = new RemoteWebDriver(new Uri("http://selenium-hub:4444"), driverOptions.ToCapabilities()/*, TimeSpan.FromMinutes(3)*/);
    }

    /// <returns>Email || Username, Password</returns>
    protected Tuple<string, string> GetLoginCredentials(bool isGettingEmail = true)
    {
        return Tuple.Create(
            isGettingEmail ? Environment.GetEnvironmentVariable("EMAIL")! 
                : Environment.GetEnvironmentVariable("USERNAME")!, 
            Environment.GetEnvironmentVariable("PASSWORD")!
        );
    }

    private static Tuple<string, string> GetTokenIdentity() 
    {
        return Tuple.Create("_identity", Environment.GetEnvironmentVariable("IDENTITY")!);
    }

    private static string GetTokenDomain() 
    {
        return Environment.GetEnvironmentVariable("DOMAIN")!;
    }

    private static DateTime GetTokenExpiresAt() 
    {
        return DateTime.ParseExact(Environment.GetEnvironmentVariable("EXPIRES")!, "dd.MM.yyyy HH:mm:ss", null);
    }

    private static IWebDriver AddIdentityCookie(IWebDriver driver)
    {
        driver.Manage().Cookies.AddCookie(new Cookie(GetTokenIdentity().Item1, GetTokenIdentity().Item2, domain: GetTokenDomain(), null, expiry: GetTokenExpiresAt(), secure: false, true, sameSite: "Lax"));
        return driver;
    } 


    //TESTS
    [HttpGet("login-email")]
    public IActionResult LoginWithEmail()
    {
        var credentials = GetLoginCredentials();
        driver.Url = "https://musescore.com/user/login";
        
        var emailInput = driver.FindElement(By.Id("username"));
        emailInput.Clear();
        emailInput.SendKeys(credentials.Item1);

        var passwordInput = driver.FindElement(By.Id("password"));
        passwordInput.Clear();
        passwordInput.SendKeys(credentials.Item2);

        var submitButton = driver.FindElement(By.Name("op"));
        submitButton.Click();

        if (driver.Url != "https://musescore.com/dashboard")
            return Ok("failed");

        try {
            driver.FindElement(By.ClassName("TtlUw TtlUw DHBDz HFvdW plVkZ wXNik utani u_VDg"));
            return Ok("failed");
        } catch {}

        return Ok("passed");
    }

    [HttpGet("login-username")]
    public IActionResult LoginWithUsername()
    {
        var credentials = GetLoginCredentials(isGettingEmail: false);
        driver.Url = "https://musescore.com/user/login";
        
        var usernameInput = driver.FindElement(By.Id("username"));
        usernameInput.Clear();
        usernameInput.SendKeys(credentials.Item1);

        var passwordInput = driver.FindElement(By.Id("password"));
        passwordInput.Clear();
        passwordInput.SendKeys(credentials.Item2);

        var submitButton = driver.FindElement(By.Name("op"));
        submitButton.Click();

        if (driver.Url != "https://musescore.com/dashboard")
            return Ok("failed");

        try {
            driver.FindElement(By.ClassName("TtlUw TtlUw DHBDz HFvdW plVkZ wXNik utani u_VDg"));
            return Ok("failed");
        } catch {}

        return Ok("passed");
    }

    [HttpGet("filter-scores")]
    public IActionResult FilterScores()
    {
        driver.Url = "https://musescore.com/dashboard";
        driver = AddIdentityCookie(driver);
        string text;
        try {
            driver.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(10));
            driver.FindElement(By.XPath("//*[@id=\"header-block\"]/nav/div[5]/button")).Click();
            
            var searchInput = driver.FindElement(By.CssSelector("#search-wrapper > form > div.wMbIR > div > input"));
            searchInput.Clear();
            searchInput.SendKeys("Clair de lune");

            var submitButton = driver.FindElement(By.XPath("//*[@id=\"search-wrapper\"]/form/span/button"));
            submitButton.Click();
            driver.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(10));
        
            var label = driver.FindElement(By.ClassName("csb5I"));
            text = label.Text;
            if(text.Contains("clair de lune", StringComparison.CurrentCultureIgnoreCase))
                return Ok("passed");
        } finally {
            driver.Quit();
        }

        return Ok($"failed: {text}");
    }

    [HttpGet("filter-scores-with-checkbox")]
    public IActionResult FilterScoresWithCheckbox()
    {
        driver.Url = "https://musescore.com/dashboard";
        driver = AddIdentityCookie(driver);
        string text;
        string level;
        try {
            driver.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(10));
            driver.FindElement(By.Id("Scores")).Click();
            driver.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(10));
            driver.FindElement(By.XPath("//*[@id=\"header-block\"]/nav/div[5]/button")).Click();

            var searchInput = driver.FindElement(By.CssSelector("#search-wrapper > form > div.wMbIR > div > input"));
            searchInput.Clear();
            searchInput.SendKeys("Clair de lune");

            var submitButton = driver.FindElement(By.XPath("//*[@id=\"search-wrapper\"]/form/span/button"));
            submitButton.Click();
            driver.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(10));
        
            var label = driver.FindElement(By.ClassName("csb5I"));
            text = label.Text;

            driver.FindElement(By.XPath("//*[@id=\"1\"]")).Click();
            driver.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(5));

            label = driver.FindElement(By.ClassName("Ze3vP"));
            level = label.Text;

            if(text.Contains("clair de lune", StringComparison.CurrentCultureIgnoreCase) 
            && level.Contains("beginner", StringComparison.CurrentCultureIgnoreCase))
                return Ok("passed");
        } finally {
            driver.Quit();
        }

        return Ok($"failed: {text}. {level}");
    }

    [HttpGet("open-score")]
    public IActionResult OpenScore()
    {
        driver.Url = "https://musescore.com/dashboard";
        driver = AddIdentityCookie(driver);
        string text;
        try {
            driver.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(10));
            driver.FindElement(By.XPath("//*[@id=\"header-block\"]/nav/div[5]/button")).Click();
            
            var searchInput = driver.FindElement(By.CssSelector("#search-wrapper > form > div.wMbIR > div > input"));
            searchInput.Clear();
            searchInput.SendKeys("Clair de lune");

            var submitButton = driver.FindElement(By.XPath("//*[@id=\"search-wrapper\"]/form/span/button"));
            submitButton.Click();
            driver.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(10));

            var scoreLabel = driver.FindElement(By.ClassName("csb5I"));
            scoreLabel.Click();
            driver.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(7));
        
            var label = driver.FindElement(By.XPath("//*[@id=\"aside-container-unique\"]/h1"));
            text = label.Text;
            if(text.Contains("clair de lune", StringComparison.CurrentCultureIgnoreCase))
                return Ok("passed");
        } finally {
            driver.Quit();
        }

        return Ok($"failed: {text}");
    }
}
