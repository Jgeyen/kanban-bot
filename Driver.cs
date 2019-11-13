using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            var skills = worker?.FindElements(By.CssSelector(skillsSelector)).Select(s => (s.GetAttribute("data-level"),s.GetAttribute("class")));

            foreach(var skill in skills)(
                var level = skill.GetElementAttributeTextById(skillId, "data-level");
                var skillClass = skill.GetElementAttributeTextById(skillId, "class");
            )
            var skillid = skills[0].GetAttribute("data-");
            return _driver.FindElementsById(workerId)[0]?.FindElements(By.CssSelector(skillsSelector)).Select(s => s.GetAttribute("id")).ToList();
        }
        public IWebElement GetElementById(string id)
        {
            return _driver.FindElementsById(id)[0];
        }
        public IWebElement GetElementByCss(string css)
        {
            return _driver.FindElementsByCssSelector(css)[0];
        }
        public bool IsElementPresentByCss(string css)
        {
            return _driver.FindElementsByCssSelector(css).Any();
        }
        public bool IsElementPresentById(string id)
        {
            return _driver.FindElementsById(id).Any();
        }
        public string GetElementAttributeTextById(string id, string attribute)
        {
            return GetElementAttributeText(By.Id(id), attribute);
        }

        public string GetElementAttributeTextByCss(string css, string attribute)
        {
            return GetElementAttributeText(By.CssSelector(css), attribute);
        }

        private string GetElementAttributeText(By by, string attribute)
        {
            var text = "";
            for (var i = 0; i < retries; i++)
            {
                try
                {
                    text = _driver.FindElements(by)[0]?.GetAttribute(attribute) ?? "";
                }
                catch (StaleElementReferenceException) //This happens randomly and without warning. Best solution is to try again. Odds decrease each attempt.
                {
                    Thread.Sleep(10);
                }
            }
            return text;
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
                    text = _driver.FindElements(by)[0]?.Text ?? "";
                }
                catch (StaleElementReferenceException) //This happens randomly and without warning. Best solution is to try again. Odds decrease each attempt.
                {
                    Thread.Sleep(10);
                }
            }
            return text;
        }
        public void ClickItemById(string id)
        {
            ClickItem(By.Id(id));
        }
        public void ClickItemByCss(string css)
        {
            ClickItem(By.CssSelector(css));
        }
        private void ClickItem(By by)
        {
            for (var i = 0; i < retries; i++)
            {
                try
                {
                    _driver.FindElements(by)[0]?.Click();
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