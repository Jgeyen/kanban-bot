using System;

namespace kanban_bot {
    public class Actions {
        private WorkerPool _workers;
        private KanbanBoard _board;
        private Store _store;
        private State _state;
        public Actions(WorkerPool workers, KanbanBoard board, Store store, State state) {
            _workers = workers;
            _board = board;
            _store = store;
            _state = state;
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
            if (!_state.CanHireDev) return;
            HireWorker(WorkerTypes.dev);
        }
        public void HireTester() {
            if (!_state.CanHireTest) return;
            HireWorker(WorkerTypes.test);
        }
        public void HireBa() {
            if (!_state.CanHireBa) return;
            HireWorker(WorkerTypes.ba);
        }
        private void HireWorker(WorkerTypes type) {
            _store.HireWorker(type);
            _state.UpdateWorkerInformation();
        }
        public void UpgradeDev() {
            if (!_state.CanUpgradeDev) return;

            UpgradeWorker(WorkerTypes.dev);
        }
        public void UpgradeTester() {
            if (!_state.CanUpgradeTest) return;

            UpgradeWorker(WorkerTypes.test);
        }
        public void UpgradeBa() {
            if (!_state.CanUpgradeBa) return;

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

            _store.PurchaseStoreItem(storeItemType);
            _store.GoToKanban();
            _board.SelectPurchasedStoreItem(type);
            _workers.GetLowestSkillWorker(type)?.SelectWithWait(TimeSpan.FromSeconds(10));
            _state.UpdateWorkerInformation();

        }
        public void AddProject() {
            _board.AddProject();
        }
    }
}