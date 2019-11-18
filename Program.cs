using System;
using System.Diagnostics;
using System.Threading;
using OpenQA.Selenium.Chrome;

namespace kanban_bot {
    class Program {
        static void Main(string[] args) {
            Driver driver = null;
            try {
                driver = GetStarted();

                var game = new Game(driver,
                                    new AddingWorkStrategy(),
                                    new FounderStrategy(WorkerTypes.founder),
                                    new SimpleEmployeeStrategy(WorkerTypes.dev),
                                    new SimpleEmployeeStrategy(WorkerTypes.test),
                                    new SimpleEmployeeStrategy(WorkerTypes.ba));

                game.Pool.UpdateWorkers();

                var loopCount = 0;
                while (true) {
                    loopCount++;

                    game.UpdateState();
                    game.AddProject();
                    Debug.WriteLine($"About to Founder: {DateTime.Now}");
                    game.FounderWork();
                    Debug.WriteLine($"About to Dev: {DateTime.Now}");
                    game.DeveloperWork();
                    Debug.WriteLine($"About to Test: {DateTime.Now}");
                    game.TesterWork();
                    Debug.WriteLine($"About to BA: {DateTime.Now}");
                    game.BaWork();
                    if (loopCount % 25 == 0) {
                        game.HireDeveloper();
                        game.HireTester();
                        game.HireBa();
                    }
                    if (loopCount % 50 == 0) {
                        game.UpgradeDeveloper();
                        game.UpgradeTester();
                        game.UpgradeBa();
                    }
                }
            } finally {
                driver?.Dispose();
            }
        }

        private static Driver GetStarted() {
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