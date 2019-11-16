using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace kanban_bot
{
    public interface ISimpleStrategy
    {
        void ExecuteStrategy(WorkerPool pool, KanbanBoard board, Store store);
    }

    public interface IEmployeeStrategy
    {
        void Hire(WorkerPool pool, KanbanBoard board, Store store);
        void DoWork(WorkerPool pool, KanbanBoard board, Store store);
        void Upgrade(WorkerPool pool, KanbanBoard board, Store store);
    }

    public class AddingWorkStrategy : ISimpleStrategy
    {
        public void ExecuteStrategy(WorkerPool workerPool, KanbanBoard board, Store store)
        {
            if (
                !board.TotalStories(StoryTypes.ba).Any() &&
                board.AvailableStories(StoryTypes.dev).Count() < (workerPool.Workers.Where(w => w.Type == WorkerTypes.dev).Count() + 1) * 2 &&
                store.TotalMoneyAvailable() > -200)
            {
                board.AddProject();
            }
        }
    }
    public class SimpleEmployeeStrategy : IEmployeeStrategy
    {
        private WorkerTypes _workerType;
        public SimpleEmployeeStrategy(WorkerTypes workerType)
        {
            _workerType = workerType;
        }
        public void Hire(WorkerPool pool, KanbanBoard board, Store store)
        {
            if (ShouldHire(pool, board, store))
            {
                store.HireWorker(_workerType);
                pool.UpdateWorkers();
            }
        }

        private bool ShouldHire(WorkerPool pool, KanbanBoard board, Store store)
        {
            var workerCount = pool.Workers.Count(w => w.Type == _workerType);
            var baCount = pool.Workers.Count(w => w.Type == WorkerTypes.ba);
            var devCount = pool.Workers.Count(w => w.Type == WorkerTypes.dev);
            var testCount = pool.Workers.Count(w => w.Type == WorkerTypes.test);
            var anyLowSkillWorkers = pool.Workers.Any(w => w.Type == _workerType && w.SkillLevel < workerCount * 2);
            var who = pool.Workers.Where(w => w.Type == _workerType && w.SkillLevel < workerCount * 5).ToList();
            var result = workerCount < 4 &&
            (workerCount <= baCount ||
            workerCount <= devCount ||
            workerCount <= testCount) &&
            !anyLowSkillWorkers &&
            store.IsHireWorkerButtonAvailable(_workerType) &&
            store.TotalMoneyAvailable() > store.WorkerPurchaseCost(_workerType);

            Debug.WriteLine($"Hire Decision was: {result} with the following information:");
            Debug.WriteLine($"WorkerCount:{workerCount}");
            Debug.WriteLine($"baCount:{baCount}");
            Debug.WriteLine($"devCount:{devCount}");
            Debug.WriteLine($"testCount:{testCount}");
            Debug.WriteLine($"anyLowSkillWorkers:{anyLowSkillWorkers}");
            Debug.WriteLine($"IsHireWorkerButtonAvailable:{store.IsHireWorkerButtonAvailable(_workerType)}");
            Debug.WriteLine($"TotalMoneyAvailable:{store.TotalMoneyAvailable()}");
            Debug.WriteLine($"WorkerPurchaseCost:{store.WorkerPurchaseCost(_workerType)}");

            return result;

        }
        public void DoWork(WorkerPool pool, KanbanBoard board, Store store)
        {
            foreach (var worker in pool.Workers.Where(w => w.Type == _workerType && !w.isBusy()).OrderByDescending(w => w.SkillLevel))
            {
                var work = board.FindNextWork((StoryTypes)_workerType);
                if (work != null)
                {
                    work.Select();
                    worker.Select();
                }
            }
        }

        public void Upgrade(WorkerPool pool, KanbanBoard board, Store store)
        {
            store.GoToStore();
            var workers = pool.Workers.Where(w => w.Type == _workerType);
            var storeItemType = _workerType switch
            {
                WorkerTypes.dev => StoreItems.UpskillDeveloper,
                WorkerTypes.test => StoreItems.UpskillTester,
                WorkerTypes.ba => StoreItems.UpskillBA,
                WorkerTypes.founder => StoreItems.UpskillDeveloper
            };

            var item = store.RetrieveStoreItem(storeItemType);
            if (workers.Count() >= 1 &&
                workers.Where(w => w.SkillLevel < workers.Count() * 2).Any() &&
                item.Cost() < store.TotalMoneyAvailable())
            {
                store.PurchaseStoreItem(item);
                store.GoToKanban();
                board.SelectPurchasedStoreItem(_workerType);
                workers.OrderBy(w => w.SkillLevel).FirstOrDefault()?.SelectWithWait(TimeSpan.FromSeconds(10));
                pool.UpdateWorkers();
            }
            //Return from store page
            store.GoToKanban();
        }
    }

    public class FounderStrategy : IEmployeeStrategy
    {
        private WorkerTypes _workerType;
        public FounderStrategy(WorkerTypes workerType)
        {
            _workerType = workerType;
        }
        public void Hire(WorkerPool pool, KanbanBoard board, Store store)
        {
            return;
        }

        public void DoWork(WorkerPool pool, KanbanBoard board, Store store)
        {
            var founder = pool.Workers.First(w => w.Type == WorkerTypes.founder);
            var workFound = false;
            var devs = pool.Workers.Count(w => w.Type == WorkerTypes.dev);
            var testers = pool.Workers.Count(w => w.Type == WorkerTypes.test);
            var bas = pool.Workers.Count(w => w.Type == WorkerTypes.ba);
            var avg = (devs + testers + bas) /3;

            var doDevWork = devs <= avg;
            var doTestWork =  testers <= avg;
            var doBaWork =  bas <= avg;

            if (!founder.isBusy())
            {
                if (doBaWork && pool.AnyIdleWorkers(WorkerTypes.dev))
                {
                    var work = board.FindNextWork(StoryTypes.ba);
                    if (work != null)
                    {
                        work.Select();
                        founder.Select();
                        workFound = true;
                    }
                }
                if (doDevWork && !workFound && pool.AnyIdleWorkers(WorkerTypes.test))
                {
                    var work = board.FindNextWork(StoryTypes.dev);
                    if (work != null)
                    {
                        work.Select();
                        founder.Select();
                        workFound = true;
                    }
                }
                if (!workFound)
                {
                    var work = (doTestWork ? board.FindNextWork(StoryTypes.test): null) ??
                                (doDevWork ? board.FindNextWork(StoryTypes.dev) : null) ??
                                (doTestWork ? board.FindNextWork(StoryTypes.ba): null);

                    if (work != null)
                    {
                        work.Select();
                        founder.Select();
                    }
                }
            }
        }

        public void Upgrade(WorkerPool pool, KanbanBoard board, Store store)
        {
            throw new System.NotImplementedException();
        }
    }
}
