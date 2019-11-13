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
            return ExtractMoney(_driver.GetElementTextById("money")) ?? int.MinValue;
        }

        public int WorkerPurchaseCost(WorkerTypes workerType)
        {
            var type = Enum.GetName(typeof(WorkerTypes), workerType);

            
            return ExtractMoney(_driver.GetWorkerButtonCost(workerType)) ?? int.MaxValue;
        }

        public void HireWorker(WorkerTypes workerType)
        {
            _driver.ClickHireWorker(workerType);
        }

        public bool IsHireWorkerButtonAvailable(WorkerTypes workerType)
        {
            return _driver.IsHireWorkerButtonAvailable(workerType);
        }
        private int? ExtractMoney(string text)
        {
            var moneyText = Regex.Match(text, @"-?\d+").Value;
            return int.TryParse(moneyText, out int result) ? (int?)result : null;
        }
    }
}