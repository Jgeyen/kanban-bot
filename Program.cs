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

                var game = new Game(_driver,
                                    new AddingWorkStrategy(),
                                    new FounderStrategy(WorkerTypes.founder),
                                    new SimpleEmployeeStrategy(WorkerTypes.dev),
                                    new SimpleEmployeeStrategy(WorkerTypes.test),
                                    new SimpleEmployeeStrategy(WorkerTypes.ba));

                game.Pool.UpdateWorkers();

                while (true)
                {
                    game.AddProject();
                    Thread.Sleep(10);

                    game.HireDeveloper();
                    Thread.Sleep(10);

                    game.HireTester();
                    Thread.Sleep(10);

                    game.HireBa();
                    Thread.Sleep(10);

                    game.DeveloperWork();
                    Thread.Sleep(10);

                    game.TesterWork();
                    Thread.Sleep(10);

                    game.BaWork();
                    Thread.Sleep(10);

                    game.FounderWork();
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