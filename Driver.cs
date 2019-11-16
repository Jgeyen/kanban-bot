using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace kanban_bot {
    public class Driver : IDisposable {
        private const string skillsSelector = "div.skills>span.skill";

        private const int retries = 3;
        private ChromeDriver _driver;

        public Driver(ChromeDriver driver) {
            _driver = driver;
        }
        public List<string> GetAvailableStoryIds(StoryTypes type) {
            return GetIds(By.CssSelector($"span.story.{type}:not(.busy)"));
        }
        public string GetNextStoryId(StoryTypes type) {
            return _driver.FindElementsByCssSelector($"span.story.{type}:not(.busy)").FirstOrDefault()?.GetAttribute("id") ?? "";
        }
        public List<string> GetTotalStoryIds(StoryTypes type) {
            return GetIds(By.CssSelector($"span.story.{type}"));
        }
        public StoryTypes GetStoryType(string id) {
            var storyClass = _driver.FindElementById(id).GetAttribute("class").Split(' ').Where(c => c == "dev" || c == "test" || c == "ba").FirstOrDefault();
            return storyClass switch
            {
                "ba" => StoryTypes.ba,
                "dev" => StoryTypes.dev,
                "test" => StoryTypes.test
            };
        }

        public List<string> GetWorkerIds() {
            return GetIds(By.CssSelector($".person.doer"));
        }
        public string GetIdleWorkerId(WorkerTypes type) {
            if(type == WorkerTypes.founder){
                return GetId(By.CssSelector($"#p1:not(.busy)"));
            }
            return GetId(By.CssSelector($"#people .person.doer.{type}:not(.busy)"));
        }
        private string GetId(By by){
            string id=null;
            for (var i = 0; i < retries; i++) {
                try {
                    id = _driver.FindElement(by).GetAttribute("id");
                    break;
                } catch (StaleElementReferenceException) //This happens randomly and without warning. Best solution is to try again. Odds decrease each attempt.
                  {
                    Thread.Sleep(10);
                }
            }
            return id;
        }
        private List<string> GetIds(By by) {
            List<string> ids = null;
            for (var i = 0; i < retries; i++) {
                try {
                    ids = _driver.FindElements(by).Select(s => s.GetAttribute("id")).ToList();
                    break;
                } catch (StaleElementReferenceException) //This happens randomly and without warning. Best solution is to try again. Odds decrease each attempt.
                  {
                    Thread.Sleep(10);
                }
            }

            return ids;
        }
        public List<(int level, string skillClass)> GetSkillsForWorker(string workerId) {
            List<(int level, string skillClass)> skills = null;
            for (var i = 0; i < retries; i++) {
                try {
                    var worker = _driver.FindElementsById(workerId)[0];
                    skills = worker?.FindElements(By.CssSelector(skillsSelector)).Select(s => (level: int.Parse(s.GetAttribute("data-level")), skillClass: s.GetAttribute("class"))).ToList();
                    break;
                } catch (StaleElementReferenceException) //This happens randomly and without warning. Best solution is to try again. Odds decrease each attempt.
                  {
                    Thread.Sleep(10);
                }
            }
            return skills;
        }

        public bool IsIdleWorkerAvailable(WorkerTypes workerType) {
            return _driver.FindElementsByCssSelector($"span.person.{workerType}:not(.busy):not(#p1)").Any();
        }

        public int GetIdleWorkerCount(WorkerTypes workerType){
            return _driver.FindElementsByCssSelector($"person.doer.{workerType}:not(.busy):not(#p1)").Count();
        }
        public bool IsHireWorkerButtonAvailable(WorkerTypes workerType) {
            return _driver.FindElementsByCssSelector($"div.getPerson.{workerType}:not(.hidden)").Any();
        }
        public bool IsStoreItemAvailable(string id) {
            return _driver.FindElementsByCssSelector($"#items>.storeItem-catalog.item-enabled>#{id}").Any();
        }
        public string StoreItemCost(string id) {
            return GetElementText(By.CssSelector($"#items>.storeItem-catalog.item-enabled>#{id}"));
        }
        public void PurchaseStoreItem(string id) {
            ClickItemById(id);
        }

        public void ClickPurchasedItem(WorkerTypes type) {
            ClickItem(By.CssSelector($".storeItem.receiver.{type}"));
        }
        public void GoToStore() {
            ClickItem(By.CssSelector(".button.visitStore"));
        }
        public void GoToKanban() {
            ClickItemById("closeStore");
        }
        public string GetHireWorkerCost(WorkerTypes workerType) {
            return GetElementText(By.CssSelector($"div.getPerson.{workerType}:not(.hidden)"));
        }
        public string GetNewProjectCost() {
            return GetElementText(By.Id("getLead"));
        }
        public bool GetClassExistsInElementById(string id, string classToCheck){
            return GetElementAttributeTextById(id, "class").Contains(classToCheck);
        }
         public bool GetClassExistsInElementByCss(string css, string classToCheck){
            return GetElementAttributeTextByCss(css, "class").Contains(classToCheck);
        }
        public string GetElementAttributeTextById(string id, string attribute) {
            return GetElementAttributeText(By.Id(id), attribute);
        }
        public string GetElementAttributeTextByCss(string css, string attribute) {
            return GetElementAttributeText(By.CssSelector(css), attribute);
        }

        public int? GetIntElementAttributeTextByCss(string css, string attribute) {
            return int.TryParse(GetElementAttributeText(By.CssSelector(css), attribute), out int response) ? (int?)response : null;
        }
        private string GetElementAttributeText(By by, string attribute) {
            var text = "";
            for (var i = 0; i < retries; i++) {
                try {
                    text = _driver.FindElements(by).FirstOrDefault()?.GetAttribute(attribute) ?? "";
                    break;
                } catch (StaleElementReferenceException) //This happens randomly and without warning. Best solution is to try again. Odds decrease each attempt.
                  {
                    Thread.Sleep(10);
                }
            }
            return text;
        }


        public string GetElementTextById(string id) {
            return GetElementText(By.Id(id));
        }

        public string GetElementTextByCss(string css) {
            return GetElementText(By.CssSelector(css));
        }
        private string GetElementText(By by) {
            var text = "";
            for (var i = 0; i < retries; i++) {
                try {
                    var bobg = _driver.FindElements(by);

                    var bob = _driver.FindElements(by).FirstOrDefault();
                    var tedxt = _driver.FindElements(by).FirstOrDefault()?.Text;
                    text = _driver.FindElements(by).FirstOrDefault()?.Text ?? "";
                    break;
                } catch (StaleElementReferenceException) //This happens randomly and without warning. Best solution is to try again. Odds decrease each attempt.
                  {
                    Thread.Sleep(10);
                }
            }
            return text;
        }
        public void ClickHireWorker(WorkerTypes workerType) {
            ClickItem(By.CssSelector(($"div.getPerson.{workerType}:not(.hidden)")));
        }
        public void ClickItemById(string id) {
            ClickItem(By.Id(id));
        }
        private void ClickItem(By by) {
            for (var i = 0; i < retries; i++) {
                try {
                    _driver.FindElements(by).FirstOrDefault()?.Click();
                    break;
                } catch {
                    Thread.Sleep(10);
                }
            }
        }
        public void Dispose() {
            _driver.Dispose();
        }
    }
}