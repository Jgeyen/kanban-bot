using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace kanban_bot {
    public enum WorkerTypes {
        ba,
        dev,
        test,
        founder
    }

    public class WorkerPool {
        public List<Worker> Workers = new List<Worker>();
        private Driver _driver;

        public WorkerPool(Driver driver) {
            _driver = driver;
        }

        public void UpdateWorkers() {
            Workers = GetWorkers();
        }
        private List<Worker> GetWorkers() {
            var workers = new List<Worker>();
            var workerIds = _driver.GetWorkerIds();
            foreach (var workerId in workerIds) {
                workers.Add(new Worker(workerId, _driver));
            }
            return workers;
        }

        public bool AnyIdleWorkers(WorkerTypes workerType) {
            return _driver.IsIdleWorkerAvailable(workerType);
        }
        public Worker GetIdleWorker(WorkerTypes workerType) {
            var workerId = _driver.GetIdleWorkerId(workerType);
            return workerId != null ? new Worker(workerId, _driver) : null;
        }
    }

    public class Worker {
        private Driver _driver = null;
        private List<Skill> _skillz;
        private string _id;
        public int SkillLevel => _skillz.FirstOrDefault(s => s.Type == (Skill.SkillType)Type)?.Level ?? 0;
        public WorkerTypes Type => _skillz.Count > 1 ? WorkerTypes.founder : (WorkerTypes)_skillz.First().Type;

        public bool isBusy() {
            return _driver.GetElementAttributeTextById(_id, "class").Contains("busy");
        }

        public Worker(string id, Driver driver) {
            _driver = driver;
            _id = id;

            _skillz = new List<Skill>();
            var skills = _driver.GetSkillsForWorker(id);

            foreach (var skill in skills) {
                if (skill.skillClass.Contains("dev")) _skillz.Add(new Skill(Skill.SkillType.dev, skill.level));
                if (skill.skillClass.Contains("test")) _skillz.Add(new Skill(Skill.SkillType.test, skill.level));
                if (skill.skillClass.Contains("ba")) _skillz.Add(new Skill(Skill.SkillType.ba, skill.level));
            }
        }

        public void Select() {
            _driver.ClickItemById(_id);
        }
        public void SelectWithWait(TimeSpan timeout) {
            var endTime = DateTime.Now + timeout;
            while (isBusy() && endTime > DateTime.Now) {
                Thread.Sleep(10);
            }
            Select();
        }
    }
    public class Skill {
        public enum SkillType {
            ba,
            dev,
            test
        }
        public SkillType Type { get; private set; }
        public int Level { get; private set; }

        public Skill(SkillType type, int level) {
            Type = type;
            Level = level;
        }
    }
}