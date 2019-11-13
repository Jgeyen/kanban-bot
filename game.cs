using OpenQA.Selenium.Chrome;

namespace kanban_bot
{
    public class Game
    {
        public WorkerPool Pool;
        public KanbanBoard Board;
        public Store Store;
        private ChromeDriver _driver;
        public ISimpleStrategy ProjectStrategy;
        public IEmployeeStrategy FounderStrategy;
        public IEmployeeStrategy DeveloperStrategy;
        public IEmployeeStrategy TesterStrategy;
        public IEmployeeStrategy BaStrategy;

        public Game(ChromeDriver driver, ISimpleStrategy projectStrategy, IEmployeeStrategy founderStrategy, IEmployeeStrategy developerStrategy, IEmployeeStrategy testerStrategy, IEmployeeStrategy baStrategy)
        {
            _driver = driver;
            Pool = new WorkerPool(_driver);
            Board = new KanbanBoard(_driver);
            Store = new Store(_driver);
            ProjectStrategy = projectStrategy;
            FounderStrategy = founderStrategy;
            DeveloperStrategy = developerStrategy;
            TesterStrategy = testerStrategy;
            BaStrategy = baStrategy;
        }

        public void AddProject()
        {
            ProjectStrategy.ExecuteStrategy(Pool, Board, Store);
        }

        public void HireDeveloper()
        {
            DeveloperStrategy.Hire(Pool, Board, Store);
        }

        public void HireTester()
        {
            TesterStrategy.Hire(Pool, Board, Store);
        }
        public void HireBa()
        {
            BaStrategy.Hire(Pool, Board, Store);
        }

        public void DeveloperWork()
        {
            DeveloperStrategy.DoWork(Pool, Board, Store);
        }

        public void TesterWork()
        {
            TesterStrategy.DoWork(Pool, Board, Store);
        }
        public void BaWork()
        {
            BaStrategy.DoWork(Pool, Board, Store);
        }
        public void FounderWork()
        {
            FounderStrategy.DoWork(Pool, Board, Store);
        }
    }
}