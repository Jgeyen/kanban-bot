using System;
using System.Diagnostics;
using System.Linq;

namespace kanban_bot {
    public class State {
        private static bool _visitedStore = false;
        private static bool _devUpgradeEnabled = false;
        private static bool _testUpgradeEnabled = false;
        private static bool _baUpgradeEnabled = false;
        public int InboxStoryCount;
        public int BacklogStoryCount;
        public int DevStoryCount;
        public int TestStoryCount;
        public int DoneStoryCount;
        public bool FounderFree;
        public int DevsFree;
        public int TestersFree;
        public int BasFree;
        public int DevCount;
        public int TesterCount;
        public int BaCount;
        public int Bank;
        public int NewProjectCost;
        public int DevHireCost;
        public int TestHireCost;
        public int BaHireCost;
        public int DevUpgradeCost;
        public int TestUpgradeCost;
        public int BaUpgradeCost;

        public int DevMinLevel;
        public int TestMinLevel;
        public int BaMinLevel;

        public bool CanHireDev;
        public bool CanHireTest;
        public bool CanHireBa;

        public bool CanUpgradeDev;
        public bool CanUpgradeTest;
        public bool CanUpgradeBa;

        private WorkerPool _workers;
        private KanbanBoard _board;
        private Store _store;
        private static int loopCount = 0;
        public State(WorkerPool workers, KanbanBoard board, Store store) {
            _workers = workers;
            _board = board;
            _store = store;
        }

        public void UpdateState() {
            var start = DateTime.Now;
            InboxStoryCount = _board.InboxStoryCount;
            BacklogStoryCount = _board.BacklogStoryCount;
            DevStoryCount = _board.DevStoryCount;
            TestStoryCount = _board.TestStoryCount;
            DoneStoryCount = _board.DoneStoryCount;
            Debug.WriteLine($"Story count in: {DateTime.Now.Subtract(start).TotalMilliseconds})");
            start = DateTime.Now;

            FounderFree = _workers.Workers.Where(w => w.Id == "p1").FirstOrDefault()?.isFree ?? false;
            Debug.WriteLine($"Founder Free in: {DateTime.Now.Subtract(start).TotalMilliseconds})");
            start = DateTime.Now;

            DevsFree = _workers.IdleWorkerCount(WorkerTypes.dev);
            TestersFree = _workers.IdleWorkerCount(WorkerTypes.test);
            BasFree = _workers.IdleWorkerCount(WorkerTypes.ba);
            Debug.WriteLine($"idle count in: {DateTime.Now.Subtract(start).TotalMilliseconds})");
            start = DateTime.Now;

            Bank = _store.TotalMoneyAvailable;
            NewProjectCost = _store.NewProjectCost;
            Debug.WriteLine($"Bank and Project Cost in: {DateTime.Now.Subtract(start).TotalMilliseconds})");

            if (DevUpgradeCost == int.MaxValue || TestUpgradeCost == int.MaxValue || BaUpgradeCost == int.MaxValue) {
                UpdateWorkerUpgradeCost();
            }

            if (DevHireCost == int.MaxValue || TestHireCost == int.MaxValue || BaHireCost == int.MaxValue) {
                UpdateWorkerHireCost();
            }

            CanHireDev = DevHireCost < Bank;
            CanHireTest = TestHireCost < Bank;
            CanHireBa = BaHireCost < Bank;

            CanUpgradeDev = _devUpgradeEnabled && DevUpgradeCost < Bank;
            CanUpgradeTest = _testUpgradeEnabled &&TestUpgradeCost < Bank;
            CanUpgradeBa =_baUpgradeEnabled &&BaUpgradeCost < Bank;

            if(!_devUpgradeEnabled){
                _devUpgradeEnabled = _store.IsStoreItemAvailable(WorkerTypes.dev);
            }
            if(!_testUpgradeEnabled){
                _testUpgradeEnabled = _store.IsStoreItemAvailable(WorkerTypes.test);
            }
            if(!_baUpgradeEnabled){
                _baUpgradeEnabled = _store.IsStoreItemAvailable(WorkerTypes.ba);
            }
            loopCount++;
        }

        public void UpdateWorkerUpgradeCost() {
            if (!_visitedStore && _store.StoreAvailable) {
                _store.GoToStore();
                _store.GoToKanban();
            }
            DevUpgradeCost = _store.WorkerUpdateCost(WorkerTypes.dev);
            TestUpgradeCost = _store.WorkerUpdateCost(WorkerTypes.test);
            BaUpgradeCost = _store.WorkerUpdateCost(WorkerTypes.ba);
        }

        public void UpdateWorkerInformation() {
            _workers.UpdateWorkers();

            DevCount = _workers.Workers.Where(w => w.Type == WorkerTypes.dev).Count();
            TesterCount = _workers.Workers.Where(w => w.Type == WorkerTypes.test).Count();
            BaCount = _workers.Workers.Where(w => w.Type == WorkerTypes.ba).Count();

            DevMinLevel = _workers.GetLowestSkillLevelForWorker(WorkerTypes.dev);
            TestMinLevel = _workers.GetLowestSkillLevelForWorker(WorkerTypes.test);
            BaMinLevel = _workers.GetLowestSkillLevelForWorker(WorkerTypes.ba);

            UpdateWorkerHireCost();
            UpdateWorkerUpgradeCost();
        }

        public void UpdateWorkerHireCost() {
            DevHireCost = _store.WorkerHireCost(WorkerTypes.dev);
            TestHireCost = _store.WorkerHireCost(WorkerTypes.test);
            BaHireCost = _store.WorkerHireCost(WorkerTypes.ba);
        }
    }
}