using System;
using System.Linq;
using System.Threading;
using OpenQA.Selenium.Chrome;

namespace kanban_bot
{
    class Program
    {
        private const string _url = "https://secretgeek.github.io/devShop/";
        private static ChromeDriver _driver = null;
        static void Main(string[] args)
        {
            try
            {
                GetStarted();

                var workerPool = new WorkerPool(_driver);
                var kanbanBoard = new KanbanBoard(_driver);
                var store = new Store(_driver);

                workerPool.UpdateWorkers();

                var workers = workerPool.Workers;

                while (true)
                {
                    if (ShouldAddProject(workerPool, kanbanBoard, store))
                    {
                        KanbanBoard.AddProject();
                    }

                    var workerBeeTypes = (Enum.GetValues(typeof(WorkerTypes)).Cast<WorkerTypes>()).Where(e => e != WorkerTypes.founder).ToList();

                    foreach (var type in workerBeeTypes)
                    {
                        if (ShouldAddWorker(type, workerPool, store))
                        {
                            workerPool.AddWorker(type);
                            workers = workerPool.Workers;
                        }
                        Thread.Sleep(10);

                        foreach (var worker in workers.Where(w => w.Type == type && !w.isBusy()).OrderByDescending(w => w.SkillLevel))
                        {

                            var work = kanbanBoard.FindWork((StoryTypes)type);
                            if (work != null)
                            {
                                work.Select();
                                worker.Select();
                                break;
                            }
                        }
                    }

                    var founder = workers.First(w => w.Type == WorkerTypes.founder);
                    var workFound = false;
                    if (!founder.isBusy())
                    {
                        if (workerPool.AnyIdleWorkers(WorkerTypes.dev))
                        {
                            var work = kanbanBoard.FindWork(StoryTypes.ba);
                            if (work != null)
                            {
                                work.Select();
                                founder.Select();
                                workFound = true;
                            }
                        }
                        if (!workFound && workerPool.AnyIdleWorkers(WorkerTypes.test))
                        {
                            var work = kanbanBoard.FindWork(StoryTypes.dev);
                            if (work != null)
                            {
                                work.Select();
                                founder.Select();
                            }
                        }
                        if (!workFound)
                        {
                            var work = kanbanBoard.FindWork(StoryTypes.test) ?? kanbanBoard.FindWork(StoryTypes.dev) ?? kanbanBoard.FindWork(StoryTypes.ba);
                            if (work != null)
                            {
                                work.Select();
                                founder.Select();
                            }
                        }
                    }

                    Thread.Sleep(10);
                }
            }
            finally
            {
                _driver?.Dispose();
            }
        }

        private static void GetStarted()
        {
            var rootDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Substring(6);
            var options = new ChromeOptions();
            options.AddArgument("start-maximized");
            options.AddArgument("no-sandbox");

            _driver = new ChromeDriver(rootDir, options, TimeSpan.FromMinutes(3));

            _driver.Navigate().GoToUrl(_url);
            _driver.FindElementById("start").Click();
        }

        private static bool ShouldAddProject(WorkerPool workerPool, KanbanBoard board, Store store)
        {
            return 
                !board.AvailableStories(StoryTypes.ba).Any() && 
                board.AvailableStories(StoryTypes.dev).Count() < (workerPool.Workers.Where(w => w.Type == WorkerTypes.dev).Count() +1)*2 && 
                store.TotalMoneyAvailable() > -200;
        }

        private static bool ShouldAddWorker(WorkerTypes workerType, WorkerPool workerPool, Store store)
        {
            var workerCount = workerPool.Workers.Count(w => w.Type == workerType);
            var addWorkerButton = _driver.FindElementsByCssSelector($"div.getPerson.{workerType}:not(.hidden)");

            return
                    workerCount < 4 &&
                    (workerCount <= workerPool.Workers.Count(w => w.Type == WorkerTypes.ba) ||
                    workerCount <= workerPool.Workers.Count(w => w.Type == WorkerTypes.dev) ||
                    workerCount <= workerPool.Workers.Count(w => w.Type == WorkerTypes.test)) &&
                    addWorkerButton.Any() &&
                    store.TotalMoneyAvailable() > store.WorkerPurchaseCost(workerType);
        }
        // private static bool ShouldUpgradeWorker(WorkerTypes workerType)
        // {

        //     var workers = TotalWorkers(workerType).Count();

        //     var addWorkerButton = _driver.FindElementsByCssSelector($"div.getPerson.{workerType}:not(.hidden)");

        //     return
        //             workers < 4 &&
        //             (workers <= TotalWorkers(WorkerTypes.ba).Count() ||
        //             workers <= TotalWorkers(WorkerTypes.dev).Count() ||
        //             workers <= TotalWorkers(WorkerTypes.test).Count()) &&
        //             addWorkerButton.Any() &&
        //             TotalMoneyAvailable() > WorkerPurchaseCost(workerType);
        // }
    }
}