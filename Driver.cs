using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace kanban_bot
{
    public class Driver : IDisposable
    {
        private const string skillsSelector = "div.skills>span.skill";

        private const int retries = 3;
        private ChromeDriver _driver;

        public Driver(ChromeDriver driver)
        {
            _driver = driver;
        }
        public List<string> GetStoryIds(StoryTypes type)
        {
            return _driver.FindElementsByCssSelector($"span.story.{type}:not(.busy)").Select(s => s.GetAttribute("id")).ToList();
        }
        public List<string> GetWorkerIds()
        {
            return _driver.FindElementsByCssSelector($".person.doer").Select(s => s.GetAttribute("id")).ToList();
        }
        public List<(int level, string skillClass)> GetSkillsForWorker(string workerId)
        {
            var worker = _driver.FindElementsById(workerId)[0];
            var skills = worker?.FindElements(By.CssSelector(skillsSelector)).Select(s => (level: int.Parse(s.GetAttribute("data-level")), skillClass: s.GetAttribute("class"))).ToList();

            return skills;
        }

        public bool IsIdleWorkerAvailable(WorkerTypes workerType)
        {
            var type = Enum.GetName(typeof(WorkerTypes), workerType);

            return _driver.FindElementsByCssSelector($"span.person.{type}:not(.busy):not(#p1)").Any();
        }
        public bool IsHireWorkerButtonAvailable(WorkerTypes workerType)
        {
            return _driver.FindElementsByCssSelector($"div.getPerson.{workerType}:not(.hidden)").Any();
        }

        public string GetElementAttributeTextById(string id, string attribute)
        {
            return GetElementAttributeText(By.Id(id), attribute);
        }

        private string GetElementAttributeText(By by, string attribute)
        {
            var text = "";
            for (var i = 0; i < retries; i++)
            {
                try
                {
                    text = _driver.FindElements(by).FirstOrDefault()?.GetAttribute(attribute) ?? "";
                }
                catch (StaleElementReferenceException) //This happens randomly and without warning. Best solution is to try again. Odds decrease each attempt.
                {
                    Thread.Sleep(10);
                }
            }
            return text;
        }

        public string GetWorkerButtonCost(WorkerTypes workerType)
        {
            return GetElementText(By.CssSelector($"div.getPerson.{workerType}:not(.hidden)"));
        }
        public string GetElementTextById(string id)
        {
            return GetElementText(By.Id(id));
        }

        public string GetElementTextByCss(string css)
        {
            return GetElementText(By.CssSelector(css));
        }


        private string GetElementText(By by)
        {
            var text = "";
            for (var i = 0; i < retries; i++)
            {
                try
                {
                    text = _driver.FindElements(by).FirstOrDefault()?.Text ?? "";
                }
                catch (StaleElementReferenceException) //This happens randomly and without warning. Best solution is to try again. Odds decrease each attempt.
                {
                    Thread.Sleep(10);
                }
            }
            return text;
        }
        public void ClickHireWorker(WorkerTypes workerType)
        {
            ClickItem(By.CssSelector(($"div.getPerson.{workerType}:not(.hidden)")));
        }
        public void ClickItemById(string id)
        {
            ClickItem(By.Id(id));
        }
        private void ClickItem(By by)
        {
            for (var i = 0; i < retries; i++)
            {
                try
                {
                    _driver.FindElements(by).FirstOrDefault()?.Click();
                    break;
                }
                catch
                {
                    Thread.Sleep(10);
                }
            }
        }
        public void Dispose()
        {
            _driver.Dispose();
        }
    }
}