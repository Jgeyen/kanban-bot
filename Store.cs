using System;
using System.Text.RegularExpressions;

namespace kanban_bot
{
    public enum StoreItems
    {
        UpskillTester = 50,
        UpskillDeveloper = 20,
        UpskillBA = 107,
        MechanicalKeyboard = 12

    }
    public class Store
    {
        private static Driver _driver;

        public Store(Driver driver)
        {
            _driver = driver;
        }

        public StoreItem RetrieveStoreItem(StoreItems item)
        {
            return new StoreItem(_driver, item);
        }

        public void PurchaseStoreItem(StoreItem item)
        {
            //Click on item to buy
            item.Purchase();
        }

        public void GoToStore()
        {
            _driver.GoToStore();
        }
        public void GoToKanban()
        {
            _driver.GoToKanban();
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
        public static int? ExtractMoney(string text)
        {
            var moneyText = Regex.Match(text, @"-?\d+").Value;
            return int.TryParse(moneyText, out int result) ? (int?)result : null;
        }
    }
    public class StoreItem
    {
        private const string _storeItemIdPrefix = "store-button-";
        private string _id;
        private Driver _driver;
        public StoreItem(Driver driver, StoreItems item)
        {
            _driver = driver;
            _id = $"store-button-{(int)item}";
        }
        public bool IsAvailable()
        {
            return _driver.IsStoreItemAvailable(_id);
        }
        public int Cost()
        {
            return Store.ExtractMoney(_driver.StoreItemCost(_id)) ?? int.MaxValue;
        }
        public void Purchase()
        {
            _driver.PurchaseStoreItem(_id);
        }
    }
}