using System;
using System.Text.RegularExpressions;

namespace kanban_bot
{
    public class Store
    {
        private static Driver _driver;

        public Store(Driver driver)
        {
            _driver = driver;
        }
        public int TotalMoneyAvailable()
        {
            return ExtractMoney(_driver.GetElementTextById("money"));
        }

        public int WorkerPurchaseCost(WorkerTypes workerType)
        {
            var type = Enum.GetName(typeof(WorkerTypes), workerType);

            var costText = _driver.GetElementTextByCss($"div.getPerson.{type}:not(.hidden)");
            return costText == "" ? ExtractMoney(costText) : int.MaxValue;
        }

        public void HireWorker(WorkerTypes workerType)
        {
            _driver.ClickItemByCss($"div.getPerson.{workerType}:not(.hidden)");
        }

        public bool HireWorkerButtonAvailable(WorkerTypes workerType)
        {
            return _driver.IsElementPresentByCss($"div.getPerson.{workerType}:not(.hidden)");
        }
        private int ExtractMoney(string text)
        {
            var moneyText = Regex.Match(text, @"-?\d+").Value;
            return int.Parse(moneyText);
        }
    }
}