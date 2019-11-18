using System.Collections.Generic;

namespace kanban_bot {
    public enum StoryTypes {
        ba,
        dev,
        test
    }

    public class KanbanBoard {
        public int InboxStoryCount => _driver.GetIntElementAttributeTextByCss("#ba .count", "data-count") ?? int.MinValue;
        public int BacklogStoryCount => _driver.GetIntElementAttributeTextByCss("#dev .count", "data-count") ?? int.MinValue;
        public int DevStoryCount => _driver.GetIntElementAttributeTextByCss("#dev0 .count", "data-count") ?? int.MinValue;
        public int TestStoryCount => _driver.GetIntElementAttributeTextByCss("#test .count", "data-count") ?? int.MinValue;
        public int DoneStoryCount => _driver.GetIntElementAttributeTextByCss("#done .count", "data-count") ?? int.MinValue;
        private Driver _driver;
        public KanbanBoard(Driver driver) {
            _driver = driver;
        }


        public List<string> AvailableStories(StoryTypes storyType) {
            return _driver.GetAvailableStoryIds(storyType);
        }

        public List<string> TotalStories(StoryTypes storyType) {
            return _driver.GetTotalStoryIds(storyType);
        }
        public Story FindNextWork(StoryTypes type) {
            var story = _driver.GetNextStoryId(type);
            return story != "" ? new Story(story, _driver) : null;
        }

        public void AddProject() {
            _driver.ClickItemById("getLead");
        }

        public void SelectPurchasedStoreItem(WorkerTypes type) {
            _driver.ClickPurchasedItem(type);
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
            _driver.ClickItemById(_id);
        }
    }
}
