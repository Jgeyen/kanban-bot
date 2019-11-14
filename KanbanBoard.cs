using System;
using System.Collections.Generic;
using System.Linq;

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
        private Driver _driver;
        public KanbanBoard(Driver driver)
        {
            _driver = driver;
        }

        public List<string> AvailableStories(StoryTypes storyType)
        {
            return _driver.GetAvailableStoryIds(storyType);
        }
        public List<string> TotalStories(StoryTypes storyType)
        {
            return _driver.GetTotalStoryIds(storyType);
        }
        public Story FindWork(StoryTypes type)
        {
            var stories = AvailableStories(type);
            return stories.Any() ? new Story(stories[0], _driver) : null;
        }

        public void AddProject()
        {
            _driver.ClickItemById("getLead");
        }

        public void SelectPurchasedStoreItem(WorkerTypes type)
        {
            _driver.ClickPurchasedItem(type);
        }
    }

    public class Story
    {
        private string _id;
        private Driver _driver;
        public Story(string id, Driver driver)
        {
            _id = id;
            _driver = driver;
        }

        public void Select()
        {
            _driver.ClickItemById(_id);
        }
    }
}
