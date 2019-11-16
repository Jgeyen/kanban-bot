namespace kanban_bot {
    public class Actions {
        private WorkerPool _workers;
        private KanbanBoard _board;
        private Store _store;
        public Actions(WorkerPool workers, KanbanBoard board, Store store) {
            _workers = workers;
            _board = board;
            _store = store;
        }

        public void FounderDoDevWork() {
            WorkerDoWork(WorkerTypes.founder, StoryTypes.dev);
        }
        public void FounderDoTestWork() {
            WorkerDoWork(WorkerTypes.founder, StoryTypes.test);
        }
        public void FounderDoBaWork() {
            WorkerDoWork(WorkerTypes.founder, StoryTypes.ba);
        }
        public void DoDevWork() {
            WorkerDoWork(WorkerTypes.dev, StoryTypes.dev);
        }
        public void DoTestWork() {
            WorkerDoWork(WorkerTypes.test, StoryTypes.test);
        }
        public void DoBaWork() {
            WorkerDoWork(WorkerTypes.ba, StoryTypes.ba);
        }
        private void WorkerDoWork(WorkerTypes workerType, StoryTypes storyType) {
            var worker = _workers.GetIdleWorker(workerType);
            var work = _board.FindNextWork(StoryTypes.dev);
            if (worker != null && work != null) {
                worker.Select();
                work.Select();
            }
        }
        public void HireDev() {
            _store.HireWorker(WorkerTypes.dev);
        }
        public void HireTester() {
            _store.HireWorker(WorkerTypes.test);
        }
        public void HireBa() {
            _store.HireWorker(WorkerTypes.ba);
        }
        public void UpgradeDev() {

        }
        public void UpgradeTester() {

        }
        public void UpgradeBa() {

        }
        private void UpgradeWorker(WorkerTypes type) {
            _store.GoToStore();
            var storeItemType = type switch
            {
                WorkerTypes.dev => StoreItems.UpskillDeveloper,
                WorkerTypes.test => StoreItems.UpskillTester,
                WorkerTypes.ba => StoreItems.UpskillBA,
                WorkerTypes.founder => StoreItems.UpskillDeveloper
            };
            
            var item = _store.RetrieveStoreItem(storeItemType);

            _store.PurchaseStoreItem(item);
            _store.GoToKanban();
            _board.SelectPurchasedStoreItem(type);
            _store.OrderBy(w => w.SkillLevel).FirstOrDefault()?.SelectWithWait(TimeSpan.FromSeconds(10));
        }
        public void AddProject() {
            _board.AddProject();
        }
    }
}