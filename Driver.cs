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
        public ChromeDriver CDriver;

        public Driver(ChromeDriver driver) {
            CDriver = driver;
        }

        public List<string> GetIds(By by) {
            List<string> ids = null;
            for (var i = 0; i < retries; i++) {
                try {
                    ids = CDriver.FindElements(by).Select(s => s.GetAttribute("id")).ToList();
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
                    var worker = CDriver.FindElementsById(workerId)[0];
                    skills = worker?.FindElements(By.CssSelector(skillsSelector)).Select(s => (level: int.Parse(s.GetAttribute("data-level")), skillClass: s.GetAttribute("class"))).ToList();
                    break;
                } catch (StaleElementReferenceException) //This happens randomly and without warning. Best solution is to try again. Odds decrease each attempt.
                  {
                    Thread.Sleep(10);
                }
            }
            return skills;
        }
        public int? GetLowestSkillForWorkers(WorkerTypes type) {
            int? skillLevel = 1;
            for (var i = 0; i < retries; i++) {
                try {
                    var skillLevels = CDriver.FindElementsByCssSelector($".person:not(#p1) .skill.{type}")?.Select(w => int.Parse(w.GetAttribute("data-level")));
                    skillLevel = skillLevels?.OrderBy(s => s).FirstOrDefault();
                    break;
                } catch (StaleElementReferenceException) //This happens randomly and without warning. Best solution is to try again. Odds decrease each attempt.
                  {
                    Thread.Sleep(10);
                }
            }
            return skillLevel;
        }
        
        public string GetElementAttributeText(By by, string attribute) {
            var text = "";
            for (var i = 0; i < retries; i++) {
                try {
                    text = CDriver.FindElements(by).FirstOrDefault()?.GetAttribute(attribute) ?? "";
                    break;
                } catch (StaleElementReferenceException) {
                    Thread.Sleep(10);
                }
            }
            return text;
        }

        public string GetElementText(By by) {
            var text = "";
            for (var i = 0; i < retries; i++) {
                try {
                    text = CDriver.FindElements(by).FirstOrDefault()?.Text ?? "";
                    break;
                } catch (StaleElementReferenceException) //This happens randomly and without warning. Best solution is to try again. Odds decrease each attempt.
                  {
                    Thread.Sleep(10);
                }
            }
            return text;
        }
        public void ClickItem(By by) {
            for (var i = 0; i < retries; i++) {
                try {
                    CDriver.FindElements(by).FirstOrDefault()?.Click();
                    break;
                } catch {
                    Thread.Sleep(10);
                }
            }
        }
        public void Dispose() {
            CDriver.Dispose();
        }
    }
}