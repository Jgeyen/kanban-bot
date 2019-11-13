using System;
using System.Collections.Generic;
using System.Linq;

namespace kanban_bot
{
    public enum WorkerTypes
    {
        ba,
        dev,
        test,
        founder
    }

    public class WorkerPool
    {
        public List<Worker> Workers = new List<Worker>();
        private Driver _driver;

        public WorkerPool(Driver driver)
        {
            _driver = driver;
        }

        public void UpdateWorkers()
        {
            Workers = GetWorkers();
        }
        private List<Worker> GetWorkers()
        {
            var workers = new List<Worker>();
            var workerIds = _driver.GetWorkerIds();
            foreach (var workerId in workerIds)
            {
                workers.Add(new Worker(workerId, _driver));
            }
            return workers;
        }

        public bool AnyIdleWorkers(WorkerTypes workerType)
        {
            var type = Enum.GetName(typeof(WorkerTypes), workerType);

            return _driver.IsElementPresentByCss($"span.person.{type}:not(.busy):not(#p1)");
        }

        public void AddWorker(WorkerTypes workerType)
        {
            var type = Enum.GetName(typeof(WorkerTypes), workerType);
            _driver.ClickItemByCss($"div.getPerson.{type}:not(.hidden)");
            UpdateWorkers();
        }
    }

    public class Worker
    {
        private Driver _driver = null;
        private const string skillsSelector = "div.skills>span.skill";
        private const string nameSelector = "div.name";
        private List<Skill> _skillz;
        private string _id;
        public string Name { get; private set; }
        public int SkillLevel => _skillz.FirstOrDefault(s => s.Type == (Skill.SkillType)Type)?.Level ?? 0;
        public WorkerTypes Type => _skillz.Count > 1 ? WorkerTypes.founder : (WorkerTypes)_skillz.First().Type;

        public bool isBusy()
        {
            return _driver.GetElementAttributeTextById(_id, "class").Contains("busy");
        }

        public Worker(string id, Driver driver)
        {
            _skillz = new List<Skill>();
            var skillIds = _driver.GetSkillIdsForWorker(id);

            foreach (var skillId in skillIds)
            {
                var level = _driver.GetElementAttributeTextById(skillId, "data-level");
                var skillClass = _driver.GetElementAttributeTextById(skillId, "class");

                if (skillClass.Contains("dev")) _skillz.Add(new Skill(Skill.SkillType.dev, int.Parse(level)));
                if (skillClass.Contains("test")) _skillz.Add(new Skill(Skill.SkillType.test, int.Parse(level)));
                if (skillClass.Contains("ba")) _skillz.Add(new Skill(Skill.SkillType.ba, int.Parse(level)));

            }

            Name = _driver.GetElementTextByCss(nameSelector);
            _id = id;
            _driver = driver;
        }

        public void Select()
        {
            _driver.ClickItemById(_id);
        }
    }
    public class Skill
    {
        public enum SkillType
        {
            ba,
            dev,
            test
        }
        public SkillType Type { get; private set; }
        public int Level { get; private set; }

        public Skill(SkillType type, int level)
        {
            Type = type;
            Level = level;
        }
    }
}