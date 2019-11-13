using System;
using System.Linq;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Chrome;

namespace kanban_bot
{
    public class Store
    {
        private static ChromeDriver _driver;

        public Store(ChromeDriver driver)
        {
            _driver = driver;
        }
        public int TotalMoneyAvailable()
        {
            return ExtractMoney(_driver.FindElementById("money").Text);
        }

        public int WorkerPurchaseCost(WorkerTypes workerType)
        {
            var type = Enum.GetName(typeof(WorkerTypes), workerType);

            var workerButton = _driver.FindElementsByCssSelector($"div.getPerson.{type}:not(.hidden)");
            if (workerButton.Any())
            {
                return ExtractMoney(workerButton[0].Text);
            }
            return int.MaxValue;
        }

        private int ExtractMoney(string text)
        {
            var moneyText = Regex.Match(text, @"-?\d+").Value;
            return int.Parse(moneyText);
        }
    }
}