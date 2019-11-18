using System.Diagnostics;

namespace kanban_bot {
    public interface ISimpleStrategy {
        void ExecuteStrategy(State state, Actions actions);
    }

    public interface IEmployeeStrategy {
        void Hire(State state, Actions actions);
        void DoWork(State state, Actions actions);
        void Upgrade(State state, Actions actions);
    }

    public class AddingWorkStrategy : ISimpleStrategy {
        public void ExecuteStrategy(State state, Actions actions) {
            if (
                state.InboxStoryCount == 0 &&
                state.BacklogStoryCount < (state.DevCount + 1) * 2 &&
                state.Bank > -200) {
                actions.AddProject();
            }
        }
    }
    public class SimpleEmployeeStrategy : IEmployeeStrategy {
        private WorkerTypes _type;
        public SimpleEmployeeStrategy(WorkerTypes type) {
            _type = type;
        }
        public void Hire(State state, Actions actions) {
            if (ShouldHire(state)) {
                switch (_type) {
                    case WorkerTypes.dev:
                        actions.HireDev();
                        break;
                    case WorkerTypes.test:
                        actions.HireTester();
                        break;
                    case WorkerTypes.ba:
                        actions.HireBa();
                        break;
                }
            }
        }

        private bool ShouldHire(State state) {
            int workerCount = 0, lowSkill = 0, upgradeCost = 0;

            switch (_type) {
                case WorkerTypes.dev:
                    workerCount = state.DevCount;
                    lowSkill = state.DevMinLevel;
                    upgradeCost = state.DevHireCost;
                    break;
                case WorkerTypes.test:
                    workerCount = state.TesterCount;
                    lowSkill = state.TestMinLevel;
                    upgradeCost = state.TestHireCost;
                    break;
                case WorkerTypes.ba:
                    workerCount = state.BaCount;
                    lowSkill = state.BaMinLevel;
                    upgradeCost = state.BaHireCost;
                    break;
            }

            var result = workerCount < 4 &&
            (workerCount <= state.BaCount ||
            workerCount <= state.DevCount ||
            workerCount <= state.TesterCount) &&
            !(lowSkill < workerCount * 2) &&   //Lowskill worker
            state.Bank > upgradeCost;  //Can afford to hire

            Debug.WriteLine($"Hire Decision was: {result} with the following information:{state}");

            return result;
        }
        public void DoWork(State state, Actions actions) {
            var freeWorkers = _type switch
            {
                WorkerTypes.dev => state.DevsFree,
                WorkerTypes.test => state.TestersFree,
                WorkerTypes.ba => state.BasFree
            };

            for (int i = 0; i < freeWorkers; i++) {
                switch (_type) {
                    case WorkerTypes.dev:
                        actions.DoDevWork();
                        break;
                    case WorkerTypes.test:
                        actions.DoTestWork();
                        break;
                    case WorkerTypes.ba:
                        actions.DoBaWork();
                        break;
                }
            }
        }

        public void Upgrade(State state, Actions actions) {
            switch (_type) {
                case WorkerTypes.dev:
                    if (state.DevCount >= 1 &&
                        state.DevMinLevel < state.DevCount * 2 &&
                        state.Bank > state.DevUpgradeCost) {
                        actions.UpgradeDev();
                    }
                    break;
                case WorkerTypes.test:
                    if (state.TesterCount >= 1 &&
                        state.TestMinLevel < state.TesterCount * 2 &&
                        state.Bank > state.TestUpgradeCost) {
                        actions.UpgradeTester();
                    }
                    break;
                case WorkerTypes.ba:
                    if (state.BaCount >= 1 &&
                        state.BaMinLevel < state.BaCount * 2 &&
                        state.Bank > state.BaUpgradeCost) {
                        actions.UpgradeBa();
                    }
                    break;
            }
        }
    }

    public class FounderStrategy : IEmployeeStrategy {
        private WorkerTypes _workerType;
        public FounderStrategy(WorkerTypes workerType) {
            _workerType = workerType;
        }
        public void Hire(State state, Actions actions) {
            return;
        }

        public void DoWork(State state, Actions actions) {
            var workFound = false;
            var avg = (state.DevCount + state.TesterCount + state.BaCount) / 3;

            var doDevWork = state.DevCount <= avg;
            var doTestWork = state.TesterCount <= avg;
            var doBaWork = state.BaCount <= avg;

            if (state.FounderFree) {
                if (doBaWork && state.BacklogStoryCount == 0 && state.DevCount > 0) {
                    workFound = actions.FounderDoBaWork();
                }
                if (!workFound && doDevWork && state.TestersFree > 0) {
                    workFound = actions.FounderDoDevWork();
                }
                if (!workFound && doTestWork) {
                    workFound = actions.FounderDoTestWork();
                }
                if (!workFound && doDevWork) {
                    workFound = actions.FounderDoDevWork();
                }
                if (!workFound && doBaWork) {
                    workFound = actions.FounderDoBaWork();
                }
            }
        }

        public void Upgrade(State state, Actions actions) {
            throw new System.NotImplementedException();
        }
    }
}
