namespace kanban_bot {
    public class State {
        public int InboxStoryCount;
        public int BacklogStoryCount;
        public int DevStoryCount;
        public int TestStoryCount;
        public int DoneStoryCount;
        public bool FounderFree;
        public int DevsFree;
        public int TestersFree;
        public int BasFree;
        public int Bank;
        public int NewProjectCost;
        public int HireDevCost;
        public int HireTestCost;
        public int HireBaCost;
        public int DevUpgradeCost => 0;
        public int TestUpgradeCost => 0;
        public int BaUpgradeCost => 0;

        public int MinDevLevel => 1;
        public int MinTestLevel => 1;
        public int MinBaLevel => 1;

        private Driver _driver;
        public State(Driver driver) {
            _driver = driver;
        }
        public void UpdateState() {
            InboxStoryCount = _driver.GetIntElementAttributeTextByCss("#ba .count", "data-count") ?? int.MinValue;
            BacklogStoryCount = _driver.GetIntElementAttributeTextByCss("#ba .count", "data-count") ?? int.MinValue;
            DevStoryCount = _driver.GetIntElementAttributeTextByCss("#ba .count", "data-count") ?? int.MinValue;
            TestStoryCount = _driver.GetIntElementAttributeTextByCss("#ba .count", "data-count") ?? int.MinValue;
            DoneStoryCount = _driver.GetIntElementAttributeTextByCss("#ba .count", "data-count") ?? int.MinValue;
            FounderFree = _driver.GetClassExistsInElementById("p1", "busy");
            DevsFree = _driver.GetIdleWorkerCount(WorkerTypes.dev);
            TestersFree = _driver.GetIdleWorkerCount(WorkerTypes.test);
            BasFree = _driver.GetIdleWorkerCount(WorkerTypes.ba);
            Bank = Store.ExtractMoney(_driver.GetElementTextById("money")) ?? int.MinValue;
            NewProjectCost = Store.ExtractMoney(_driver.GetNewProjectCost()) ?? int.MaxValue;
            HireDevCost = Store.ExtractMoney(_driver.GetHireWorkerCost(WorkerTypes.dev)) ?? int.MaxValue;
            HireTestCost = Store.ExtractMoney(_driver.GetHireWorkerCost(WorkerTypes.test)) ?? int.MaxValue;
            HireBaCost = Store.ExtractMoney(_driver.GetHireWorkerCost(WorkerTypes.ba)) ?? int.MaxValue;
            
        }
    }

}