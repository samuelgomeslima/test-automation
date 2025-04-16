using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Tests
{
    public class WebPageTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            driver = new ChromeDriver(options);
            const int timeOut = 10;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOut));
        }

        [Test]
        public void OpenWebPage()
        {
            const string msLearnUrl = "https://learn.microsoft.com/en-us/";
            driver.Navigate().GoToUrl(msLearnUrl);

            const string expectedTitle = "Microsoft Learn: Build skills that open doors in your career";
            Assert.That(expectedTitle == driver.Title, $"Expected title: {expectedTitle}, but got: {driver.Title}");
        }

        [Test]
        public void CheckProfileLevel()
        {
            const string profileUrl = "https://learn.microsoft.com/en-us/users/samuelglima/";
            driver.Navigate().GoToUrl(profileUrl);
            
            int level = GetAccountLevel();
            
            const int expectedMinimumLevel = 10;
            Assert.That(level > expectedMinimumLevel, $"Level should be higher than {expectedMinimumLevel}. Level: {level}");
            Console.WriteLine($"Profile Level: {level}");
        }

        private int GetAccountLevel()
        {
            const string levelElementId = "level-status-text";
            try
            {
                var levelElement = wait.Until(ExpectedConditions.ElementIsVisible(By.Id(levelElementId)));
                string levelText = levelElement.Text;
                if (int.TryParse(levelText.ToLower().Replace("level", "").Trim(), out int level))
                {
                    return level;
                }
                else
                {
                    throw new FormatException("Unable to parse level text to an integer.");
                }
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("Level element not found.");
                return -1;
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Timed out waiting for level element.");
                return -1;
            }
        }

        [TearDown]
        public void Teardown()
        {
            driver?.Quit();
        }
    }
}
