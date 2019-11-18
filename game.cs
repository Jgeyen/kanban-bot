using System;
using System.Diagnostics;

namespace kanban_bot {
    public class Game {
        public WorkerPool Pool;
        public KanbanBoard Board;
        public Store Store;
        public State State;
        public Actions Actions;
        public ISimpleStrategy ProjectStrategy;
        public IEmployeeStrategy FounderStrategy;
        public IEmployeeStrategy DeveloperStrategy;
        public IEmployeeStrategy TesterStrategy;
        public IEmployeeStrategy BaStrategy;

        public Game(Driver driver, ISimpleStrategy projectStrategy, IEmployeeStrategy founderStrategy, IEmployeeStrategy developerStrategy, IEmployeeStrategy testerStrategy, IEmployeeStrategy baStrategy) {

            Pool = new WorkerPool(driver);
            Board = new KanbanBoard(driver);
            Store = new Store(driver);
            ProjectStrategy = projectStrategy;
            FounderStrategy = founderStrategy;
            DeveloperStrategy = developerStrategy;
            TesterStrategy = testerStrategy;
            BaStrategy = baStrategy;
            State = new State(Pool, Board, Store);
            Actions = new Actions(Pool, Board, Store);
        }

        public void AddProject() {
            ProjectStrategy.ExecuteStrategy(State, Actions);
        }
        public void UpdateState() {
            var start = DateTime.Now;
            State.UpdateState();
            Debug.WriteLine($"Just updated state in: {DateTime.Now.Subtract(start).TotalMilliseconds})");
        }
        public void HireDeveloper() {
            DeveloperStrategy.Hire(State, Actions);
        }

        public void HireTester() {
            TesterStrategy.Hire(State, Actions);
        }
        public void HireBa() {
            BaStrategy.Hire(State, Actions);
        }

        public void UpgradeDeveloper() {
            DeveloperStrategy.Upgrade(State, Actions);
        }

        public void UpgradeTester() {
            TesterStrategy.Upgrade(State, Actions);
        }
        public void UpgradeBa() {
            BaStrategy.Upgrade(State, Actions);
        }

        public void DeveloperWork() {
            DeveloperStrategy.DoWork(State, Actions);
        }

        public void TesterWork() {
            TesterStrategy.DoWork(State, Actions);
        }
        public void BaWork() {
            BaStrategy.DoWork(State, Actions);
        }
        public void FounderWork() {
            FounderStrategy.DoWork(State, Actions);
        }
    }
}