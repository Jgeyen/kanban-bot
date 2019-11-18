using System;
using System.Diagnostics;
using System.Linq;

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
        public int DevCount;
        public int TesterCount;
        public int BaCount;
        public int Bank;
        public int NewProjectCost;
        public int HireDevCost;
        public int HireTestCost;
        public int HireBaCost;
        public int DevUpgradeCost;
        public int TestUpgradeCost;
        public int BaUpgradeCost;

        public int MinDevLevel;
        public int MinTestLevel;
        public int MinBaLevel;

        private WorkerPool _workers;
        private KanbanBoard _board;
        private Store _store;
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
            DevCount = _workers.Workers.Where(w => w.Type == WorkerTypes.dev).Count();
            TesterCount = _workers.Workers.Where(w => w.Type == WorkerTypes.test).Count();
            BaCount = _workers.Workers.Where(w => w.Type == WorkerTypes.ba).Count();
            Debug.WriteLine($"Dev count in: {DateTime.Now.Subtract(start).TotalMilliseconds})");
            start = DateTime.Now;
            DevsFree = _workers.IdleWorkerCount(WorkerTypes.dev);
            TestersFree = _workers.IdleWorkerCount(WorkerTypes.test);
            BasFree = _workers.IdleWorkerCount(WorkerTypes.ba);
            Debug.WriteLine($"idle count in: {DateTime.Now.Subtract(start).TotalMilliseconds})");
            start = DateTime.Now;
            Bank = _store.TotalMoneyAvailable;
            NewProjectCost = _store.NewProjectCost;
            Debug.WriteLine($"Bank and Project Cost in: {DateTime.Now.Subtract(start).TotalMilliseconds})");
            start = DateTime.Now;
            HireDevCost = _store.WorkerPurchaseCost(WorkerTypes.dev);
            HireTestCost = _store.WorkerPurchaseCost(WorkerTypes.test);
            HireBaCost = _store.WorkerPurchaseCost(WorkerTypes.ba);
            Debug.WriteLine($"Worker Hire Cost in: {DateTime.Now.Subtract(start).TotalMilliseconds})");
            start = DateTime.Now;
            DevUpgradeCost = _store.WorkerUpdateCost(WorkerTypes.dev);
            TestUpgradeCost = _store.WorkerUpdateCost(WorkerTypes.test);
            BaUpgradeCost = _store.WorkerUpdateCost(WorkerTypes.ba);
            Debug.WriteLine($"Worker Update Cost in: {DateTime.Now.Subtract(start).TotalMilliseconds})");
            start = DateTime.Now;
            MinDevLevel = _workers.GetLowestSkillLevelForWorker(WorkerTypes.dev);
            MinTestLevel = _workers.GetLowestSkillLevelForWorker(WorkerTypes.test);
            MinBaLevel = _workers.GetLowestSkillLevelForWorker(WorkerTypes.ba);
            Debug.WriteLine($"Worker lowest skill in: {DateTime.Now.Subtract(start).TotalMilliseconds})");
            start = DateTime.Now;
        }
    }

}