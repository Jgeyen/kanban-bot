
using System.Linq;

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
                !board.AvailableStories(StoryTypes.ba).Any() &&
                board.AvailableStories(StoryTypes.dev).Count() < (workerPool.Workers.Where(w => w.Type == WorkerTypes.dev).Count() + 1) * 2 &&
                store.TotalMoneyAvailable() > -200)
            {
                KanbanBoard.AddProject();
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
                    store.HireWorkerButtonAvailable(_workerType) &&
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
            throw new System.NotImplementedException();
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
