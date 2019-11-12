using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace kanban_bot
{
    public enum StoryTypes
    {
        ba,
        dev,
        test
    }

    public class KanbanBoard
    {
        private static ChromeDriver _driver;
        public KanbanBoard(ChromeDriver driver)
        {
            _driver = driver;
        }

        public ReadOnlyCollection<IWebElement> AvailableStories(StoryTypes storyType)
        {
            var type = Enum.GetName(typeof(StoryTypes), storyType);

            return _driver.FindElementsByCssSelector($"span.story.{type}:not(.busy)");
        }
        public ReadOnlyCollection<IWebElement> TotalStories(StoryTypes storyType)
        {
            var type = Enum.GetName(typeof(StoryTypes), storyType);

            return _driver.FindElementsByCssSelector($"span.story.{type}");
        }

        public Story FindWork(StoryTypes type)
        {
            var stories = AvailableStories(type);
            return stories.Any() ? new Story(stories[0], _driver): null;
        }
    }

    public class Story
    {

        private string _id;
        private ChromeDriver _driver;
        public Story(IWebElement element, ChromeDriver driver)
        {
            _id = element.GetAttribute("id");
            _driver = driver;
        }

        public void Select()
        {
            var el = _driver.FindElementsById(_id);
            if (el.Any())
            {
                var failCount = 0;
                var success = false;
                while (!success && failCount < 3)
                {
                    try
                    {
                        el[0].Click();
                        success = true;
                    }
                    catch
                    {
                        failCount++;
                        success = false;
                        Thread.Sleep(10);
                    }
                }
            }
        }
    }
}
