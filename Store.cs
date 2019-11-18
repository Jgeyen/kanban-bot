using System.Text.RegularExpressions;

namespace kanban_bot {
    public enum StoreItems {
        UpskillTester = 50,
        UpskillDeveloper = 20,
        UpskillBA = 107,
        MechanicalKeyboard = 12

    }
    public class Store {
        private static Driver _driver;

        public Store(Driver driver) {
            _driver = driver;
        }

        public StoreItem RetrieveStoreItem(StoreItems item) {
            return new StoreItem(_driver, item);
        }

        public void PurchaseStoreItem(StoreItem item) {
            //Click on item to buy
            item.Purchase();
        }

        public void GoToStore() {
            _driver.GoToStore();
        }
        public void GoToKanban() {
            _driver.GoToKanban();
        }
        public int TotalMoneyAvailable => ExtractMoney(_driver.GetElementTextById("money")) ?? int.MinValue;
        public int NewProjectCost => ExtractMoney(_driver.GetNewProjectCost()) ?? int.MinValue;

        public int WorkerPurchaseCost(WorkerTypes type) {
            return ExtractMoney(_driver.GetHireWorkerCost(type)) ?? int.MaxValue;
        }

        public void HireWorker(WorkerTypes type) {
            _driver.ClickHireWorker(type);
        }

        public bool IsHireWorkerButtonAvailable(WorkerTypes type) {
            return _driver.IsHireWorkerButtonAvailable(type);
        }
        public int WorkerUpdateCost(WorkerTypes type){
            var item = type switch{
                WorkerTypes.dev => StoreItems.UpskillDeveloper,
                WorkerTypes.test => StoreItems.UpskillTester,
                WorkerTypes.ba => StoreItems.UpskillBA
            };
            return ExtractMoney(_driver.GetElementAttributeTextByCss($"#store-button-{(int)item}", "innerText"))?? int.MaxValue;
        }
        public static int? ExtractMoney(string text) {
            var moneyText = Regex.Match(text, @"-?\d+").Value;
            return int.TryParse(moneyText, out int result) ? (int?)result : null;
        }
    }
    public class StoreItem {
        private string _id;
        private Driver _driver;
        public StoreItem(Driver driver, StoreItems item) {
            _driver = driver;
            _id = $"store-button-{(int)item}";
        }
        public int Cost => Store.ExtractMoney(_driver.StoreItemCost(_id)) ?? int.MaxValue;
        public void Purchase() {
            _driver.PurchaseStoreItem(_id);
        }
    }
}