using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

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
        private ChromeDriver _driver = null;

        public WorkerPool(ChromeDriver driver)
        {
            _driver = driver;
        }

        public void UpdateWorkers()
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    Workers = GetWorkers();
                    break;
                }
                catch (StaleElementReferenceException)
                {
                    Thread.Sleep(10);
                }
            }
        }
        private List<Worker> GetWorkers()
        {
            var workers = new List<Worker>();
            var workerElements = _driver.FindElementsByCssSelector(".person.doer");
            foreach (var worker in workerElements)
            {
                workers.Add(new Worker(worker, _driver));
            }
            return workers;
        }

        public bool AnyIdleWorkers(WorkerTypes workerType)
        {
            var type = Enum.GetName(typeof(WorkerTypes), workerType);

            return _driver.FindElementsByCssSelector($"span.person.{type}:not(.busy):not(#p1)").Any();
        }

        public void AddWorker(WorkerTypes workerType)
        {
            var type = Enum.GetName(typeof(WorkerTypes), workerType);
            _driver.FindElementByCssSelector($"div.getPerson.{type}:not(.hidden)").Click();
            UpdateWorkers();
        }


    }

    public class Worker
    {
        private ChromeDriver _driver = null;
        private const string skillsSelector = "div.skills>span.skill";
        private const string nameSelector = "div.name";
        private List<Skill> _skillz;
        private string _id;
        public string Name { get; private set; }
        public int SkillLevel => _skillz.FirstOrDefault(s => s.Type == (Skill.SkillType)Type)?.Level ?? 0;
        public WorkerTypes Type => _skillz.Count > 1 ? WorkerTypes.founder : (WorkerTypes)_skillz.First().Type;

        public bool isBusy()
        {
            var el = _driver.FindElementsById(_id);
            var busy = false;

            if (el.Any())
            {
                try
                {
                    busy = el[0].GetAttribute("class").Contains("busy");
                }
                catch (StaleElementReferenceException)
                {
                    Thread.Sleep(10);
                    el = _driver.FindElementsById(_id);
                    busy = el[0].GetAttribute("class").Contains("busy");
                }
            }
            return busy;
        }

        public Worker(IWebElement element, ChromeDriver driver)
        {
            _skillz = new List<Skill>();
            var skills = element.FindElements(By.CssSelector(skillsSelector));
            foreach (var skillEl in skills)
            {
                var levelEl = skillEl.GetAttribute("data-level");

                var skillCss = skillEl.GetAttribute("class");
                if (skillCss.Contains("dev")) _skillz.Add(new Skill(Skill.SkillType.dev, int.Parse(levelEl)));
                if (skillCss.Contains("test")) _skillz.Add(new Skill(Skill.SkillType.test, int.Parse(levelEl)));
                if (skillCss.Contains("ba")) _skillz.Add(new Skill(Skill.SkillType.ba, int.Parse(levelEl)));

            }

            Name = element.FindElements(By.CssSelector(nameSelector))[0].Text;
            _id = element.GetAttribute("id");
            _driver = driver;
        }

        public void Select()
        {
            var el = _driver.FindElementsById(_id);
            if (el.Any())
            {
                el[0].Click();
            }
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