using System;
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
            var workerCount = pool.Workers.Count(w => w.Type == _workerType);
            if (
                    workerCount < 4 &&
                    (workerCount <= pool.Workers.Count(w => w.Type == WorkerTypes.ba) ||
                    workerCount <= pool.Workers.Count(w => w.Type == WorkerTypes.dev) ||
                    workerCount <= pool.Workers.Count(w => w.Type == WorkerTypes.test)) &&
                    store.IsHireWorkerButtonAvailable(_workerType) &&
                    store.TotalMoneyAvailable() > store.WorkerPurchaseCost(_workerType))
            {
                store.HireWorker(_workerType);
                pool.UpdateWorkers();
            }
        }

        public void DoWork(WorkerPool pool, KanbanBoard board, Store store)
        {
            foreach (var worker in pool.Workers.Where(w => w.Type == _workerType && !w.isBusy()).OrderByDescending(w => w.SkillLevel))
            {
                var work = board.FindWork((StoryTypes)_workerType);
                if (work != null)
                {
                    work.Select();
                    worker.Select();
                    break;
                }
            }
        }

        public void Upgrade(WorkerPool pool, KanbanBoard board, Store store)
        {
            store.GoToStore();
            var workers = pool.Workers.Where(w => w.Type == _workerType);
            var storeItemType = _workerType switch{
                WorkerTypes.dev => StoreItems.UpskillDeveloper,
                WorkerTypes.test => StoreItems.UpskillTester,
                WorkerTypes.ba => StoreItems.UpskillBA,
                WorkerTypes.founder => StoreItems.UpskillDeveloper
            };

            var item = store.RetrieveStoreItem(storeItemType);
            if (workers.Count() >= 1 &&
                workers.Where(w => w.SkillLevel < workers.Count() * 3).Any() &&
                item.Cost() < store.TotalMoneyAvailable())
            {
                store.PurchaseStoreItem(item);
                Thread.Sleep(10);
                store.GoToKanban();
                Thread.Sleep(10);
                board.SelectPurchasedStoreItem(_workerType);
                Thread.Sleep(10);

                workers.OrderBy(w => w.SkillLevel).FirstOrDefault()?.SelectWithWait(TimeSpan.FromSeconds(10));
                
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
            if (!founder.isBusy())
            {
                if (pool.AnyIdleWorkers(WorkerTypes.dev))
                {
                    var work = board.FindWork(StoryTypes.ba);
                    if (work != null)
                    {
                        work.Select();
                        founder.Select();
                        workFound = true;
                    }
                }
                if (!workFound && pool.AnyIdleWorkers(WorkerTypes.test))
                {
                    var work = board.FindWork(StoryTypes.dev);
                    if (work != null)
                    {
                        work.Select();
                        founder.Select();
                    }
                }
                if (!workFound)
                {
                    var work = board.FindWork(StoryTypes.test) ?? board.FindWork(StoryTypes.dev) ?? board.FindWork(StoryTypes.ba);
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
