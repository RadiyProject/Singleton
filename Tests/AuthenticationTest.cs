using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Singleton.Tests;

[TestClass]
public class AuthenticationTest
{
    protected RemoteWebDriver driver;

    public AuthenticationTest() {
        var driverOptions = new ChromeOptions();
        driver = new RemoteWebDriver(new Uri("http://selenium-hub:4444"), driverOptions);
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


    //TESTS
    [TestMethod]
    public void LoginWithEmail()
    {
        var credentials = GetLoginCredentials();
        driver.Url = "https://musescore.com/user/login";
        
        var emailInput = driver.FindElementById("username");
        emailInput.Clear();
        emailInput.SendKeys(credentials.Item1);

        var passwordInput = driver.FindElementById("password");
        passwordInput.Clear();
        passwordInput.SendKeys(credentials.Item2);

        Assert.IsTrue(true);
    }

    [TestMethod]
    public void LoginWithUsername()
    {
        var credentials = GetLoginCredentials(isGettingEmail: false);
        driver.Url = "https://musescore.com/user/login";
        
        var usernameInput = driver.FindElementById("username");
        usernameInput.Clear();
        usernameInput.SendKeys(credentials.Item1);

        var passwordInput = driver.FindElementById("password");
        passwordInput.Clear();
        passwordInput.SendKeys(credentials.Item2);

        Assert.IsTrue(true);
    }
}
