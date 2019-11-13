using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace kanban_bot
{
    public class Driver :IDisposable
    {
        private ChromeDriver _driver;
        public Driver(ChromeDriver driver)
        {
            _driver = driver;
        }
        public List<string> GetStoryIds(StoryTypes type)
        {
            return _driver.FindElementsByCssSelector($"span.story.{type}:not(.busy)").Select(s => s.GetAttribute("id")).ToList();
        }
        public IWebElement GetStory(string id)
        {
            var story = _driver.FindElementsById(id);
            return story.Any() ? story[0] : null;
        }

        public void ClickItem(string id)
        {
            var el = _driver.FindElementsById(id);
            if (el.Any())
            {
                for (var i = 0; i < 4; i++)
                {
                    try
                    {
                        el[0].Click();
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(10);
                    }
                }
            }
        }

        public void Dispose()
        {
            _driver.Dispose();
        }
    }
}