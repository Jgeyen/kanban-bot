using System;

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

        public bool FounderDoDevWork() {
            return WorkerDoWork(WorkerTypes.founder, StoryTypes.dev);
        }
        public bool FounderDoTestWork() {
            return WorkerDoWork(WorkerTypes.founder, StoryTypes.test);
        }
        public bool FounderDoBaWork() {
            return WorkerDoWork(WorkerTypes.founder, StoryTypes.ba);
        }
        public bool DoDevWork() {
            return WorkerDoWork(WorkerTypes.dev, StoryTypes.dev);
        }
        public bool DoTestWork() {
            return WorkerDoWork(WorkerTypes.test, StoryTypes.test);
        }
        public bool DoBaWork() {
            return WorkerDoWork(WorkerTypes.ba, StoryTypes.ba);
        }
        private bool WorkerDoWork(WorkerTypes workerType, StoryTypes storyType) {
            var foundWork = false;
            var worker = _workers.GetIdleWorker(workerType);
            var work = _board.FindNextWork(storyType);
            if (worker != null && work != null) {
                worker.Select();
                work.Select();
                foundWork = true;
            }
            return foundWork;
        }
        public void HireDev() {
            _store.HireWorker(WorkerTypes.dev);
            _workers.UpdateWorkers();
        }
        public void HireTester() {
            _store.HireWorker(WorkerTypes.test);
            _workers.UpdateWorkers();
        }
        public void HireBa() {
            _store.HireWorker(WorkerTypes.ba);
            _workers.UpdateWorkers();
        }
        public void UpgradeDev() {
            UpgradeWorker(WorkerTypes.dev);
        }
        public void UpgradeTester() {
            UpgradeWorker(WorkerTypes.test);
        }
        public void UpgradeBa() {
            UpgradeWorker(WorkerTypes.ba);
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
            _workers.GetLowestSkillWorker(type)?.SelectWithWait(TimeSpan.FromSeconds(10));
        }
        public void AddProject() {
            _board.AddProject();
        }
    }
}