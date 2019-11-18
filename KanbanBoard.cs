using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace kanban_bot {
    public enum StoryTypes {
        ba,
        dev,
        test
    }

    public class KanbanBoard {
        public int InboxStoryCount => GetAttributeTextAsInt(By.CssSelector("#ba .count"), "data-count") ?? int.MinValue;
        public int BacklogStoryCount => GetAttributeTextAsInt(By.CssSelector("#dev .count"), "data-count") ?? int.MinValue;
        public int DevStoryCount => GetAttributeTextAsInt(By.CssSelector("#dev0 .count"), "data-count") ?? int.MinValue;
        public int TestStoryCount => GetAttributeTextAsInt(By.CssSelector("#test .count"), "data-count") ?? int.MinValue;
        public int DoneStoryCount => GetAttributeTextAsInt(By.CssSelector("#done .count"), "data-count") ?? int.MinValue;
        private Driver _driver;
        public KanbanBoard(Driver driver) {
            _driver = driver;
        }

        public int? GetAttributeTextAsInt(By by, string attribute) {
            return int.TryParse(_driver.GetElementAttributeText(by, attribute), out int response) ? (int?)response : null;
        }
        public List<string> AvailableStories(StoryTypes type) {
            return _driver.GetIds(By.CssSelector($"span.story.{type}:not(.busy)"));
        }

        public List<string> TotalStories(StoryTypes type) {
            return _driver.GetIds(By.CssSelector($"span.story.{type}"));
        }
        public Story FindNextWork(StoryTypes type) {
            var story = _driver.CDriver.FindElementsByCssSelector($"span.story.{type}:not(.busy)").FirstOrDefault()?.GetAttribute("id") ?? "";
            return story != "" ? new Story(story, _driver) : null;
        }

        public void AddProject() {
            _driver.ClickItem(By.Id("getLead"));
        }

        public void SelectPurchasedStoreItem(WorkerTypes type) {
            _driver.ClickItem(By.CssSelector($".storeItem.receiver.{type}"));
        }


    }

    public class Story {
        private string _id;
        private Driver _driver;
        public Story(string id, Driver driver) {
            _id = id;
            _driver = driver;
        }
        public void Select() {
            _driver.ClickItem(By.Id(_id));
        }
    }
}
