using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace kanban_bot
{
    class Program
    {
        

        private const string _Url = "https://test-uiautomation.verifybrand.com/";

        private static ChromeDriver _driver = null;
        static void Main(string[] args)
        {
            try
            {

                var rootDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Substring(6);
                var options = new ChromeOptions();
                options.AddArgument("start-maximized");
                options.AddArgument("no-sandbox");

                _driver = new ChromeDriver(rootDir, options, TimeSpan.FromMinutes(3));

                _driver.Navigate().GoToUrl("https://secretgeek.github.io/devShop/");
                _driver.FindElementById("start").Click();
                var workerPool = new WorkerPool(_driver);
                var kanbanBoard = new KanbanBoard(_driver);
                workerPool.UpdateWorkers();

                var workers = workerPool.Workers;

                while (true)
                {
                    if (ShouldAddProject(kanbanBoard))
                    {
                        AddProject();
                        AddProject();
                    }

                    foreach (var type in Enum.GetValues(typeof(WorkerTypes)).Cast<WorkerTypes>())
                    {
                        if (ShouldAddWorker(type, workerPool))
                        {
                            workerPool.AddWorker(type);
                            workers = workerPool.Workers;
                        }
                    }

                    foreach (var worker in workers.Where(w => w.Type == WorkerTypes.ba && !w.isBusy()).OrderByDescending(w => w.SkillLevel))
                    {
                        var work = kanbanBoard.FindWork(StoryTypes.ba);
                        if (work != null)
                        {
                            work.Select();
                            worker.Select();
                            break;
                        }
                    }

                    foreach (var worker in workers.Where(w => w.Type == WorkerTypes.test && !w.isBusy()).OrderByDescending(w => w.SkillLevel))
                    {
                        Thread.Sleep(10);

                        var work = kanbanBoard.FindWork(StoryTypes.test);
                        if (work != null)
                        {
                            work.Select();
                            worker.Select();
                            break;
                        }
                    }

                    foreach (var worker in workers.Where(w => w.Type == WorkerTypes.dev && !w.isBusy()).OrderByDescending(w => w.SkillLevel))
                    {
                        Thread.Sleep(10);

                        var work = kanbanBoard.FindWork(StoryTypes.dev);
                        if (work != null)
                        {
                            work.Select();
                            worker.Select();
                            break;
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

        

        private static bool ShouldAddProject(KanbanBoard board)
        {
            var baStories = board.TotalStories(StoryTypes.ba);
            var devStories = board.TotalStories(StoryTypes.dev);

            return !baStories.Any() && !devStories.Any() && TotalMoneyAvailable() > -200;
        }

        private static bool ShouldAddWorker(WorkerTypes workerType, WorkerPool workerPool)
        {
            var workerCount = workerPool.Workers.Count(w => w.Type== workerType);
            var addWorkerButton = _driver.FindElementsByCssSelector($"div.getPerson.{workerType}:not(.hidden)");

            return
                    workerCount < 4 &&
                    (workerCount <= workerPool.Workers.Count(w => w.Type== WorkerTypes.ba) ||
                    workerCount <= workerPool.Workers.Count(w => w.Type== WorkerTypes.dev) ||
                    workerCount <= workerPool.Workers.Count(w => w.Type== WorkerTypes.test)) &&
                    addWorkerButton.Any() &&
                    TotalMoneyAvailable() > WorkerPurchaseCost(workerType);
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

        private static void AddProject()
        {
            _driver.FindElementById("getLead").Click();
        }
        

        private static int TotalMoneyAvailable()
        {
            return ExtractMoney(_driver.FindElementById("money").Text);
        }

        private static int WorkerPurchaseCost(WorkerTypes workerType)
        {
            var type = Enum.GetName(typeof(WorkerTypes), workerType);

            var workerButton = _driver.FindElementsByCssSelector($"div.getPerson.{type}:not(.hidden)");
            if (workerButton.Any())
            {
                return ExtractMoney(workerButton[0].Text);
            }
            return int.MaxValue;
        }

        private static int ExtractMoney(string text)
        {
            var moneyText = Regex.Match(text, @"\d+").Value;
            return int.Parse(moneyText);
        }
 
        
        

        

        public class Store
        {

        }
    }
}