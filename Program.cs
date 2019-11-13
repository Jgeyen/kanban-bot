using System;
using System.Linq;
using System.Threading;
using OpenQA.Selenium.Chrome;

namespace kanban_bot
{
    class Program
    {
        static void Main(string[] args)
        {
            Driver driver = null;
            try
            {
                driver = GetStarted();

                var game = new Game(driver,
                                    new ChromeDriver(),
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
                driver?.Dispose();
            }
        }

        private static Driver GetStarted()
        {
            var rootDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Substring(6);
            var options = new ChromeOptions();
            options.AddArgument("start-maximized");
            options.AddArgument("no-sandbox");

            var driver = new ChromeDriver(rootDir, options, TimeSpan.FromMinutes(3));

            driver.Navigate().GoToUrl("https://secretgeek.github.io/devShop/");
            driver.FindElementById("start").Click();
            return new Driver(driver);
        }
    }
}