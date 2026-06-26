using System;
using System.Collections.Generic;
using System.Linq;

namespace MizzBox
{
    public class TaskManager
    {
        private TaskStorageHelper _storage;
        private ActivityLogger _logger;

        public TaskManager(TaskStorageHelper storage, ActivityLogger logger = null)
        {
            _storage = storage;
            _logger = logger;
        }

        public string AddTask(string title, string description, string reminder)
        {
            CyberTask task = _storage.AddTask(title, description, reminder);

            string logEntry = "Task added: '" + task.Title + "'";
            if (!string.IsNullOrWhiteSpace(task.Reminder))
                logEntry += " (Reminder: " + task.Reminder + ")";
            else
                logEntry += " (no reminder set)";

            _logger?.Log(logEntry);

            string confirmation = "Task added: '" + task.Title + "'";
            if (!string.IsNullOrWhiteSpace(task.Reminder))
                confirmation += "\nReminder set: " + task.Reminder;
            else
                confirmation += "\nWould you like to set a reminder for this task?";

            return confirmation;
        }

        public List<CyberTask> GetAllTasks()
        {
            return _storage.LoadTasks();
        }

        public string MarkAsComplete(int id)
        {
            bool success = _storage.MarkAsComplete(id);
            if (success)
            {
                CyberTask task = _storage.GetTaskById(id);
                _logger?.Log("Task marked complete: '" + task.Title + "'");
                return "Task '" + task.Title + "' marked as complete! ✅";
            }
            return "Task not found. Could not mark as complete.";
        }

        public string DeleteTask(int id)
        {
            CyberTask task = _storage.GetTaskById(id);
            if (task != null)
            {
                bool success = _storage.DeleteTask(id);
                if (success)
                {
                    _logger?.Log("Task deleted: '" + task.Title + "'");
                    return "Task '" + task.Title + "' deleted successfully.";
                }
            }
            return "Task not found. Could not delete.";
        }

        public string UpdateReminder(int id, string reminder)
        {
            List<CyberTask> tasks = _storage.LoadTasks();
            CyberTask task = tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                task.Reminder = reminder;
                _storage.SaveTasks(tasks);
                _logger?.Log("Reminder set for '" + task.Title + "': " + reminder);
                return "Got it! I'll remind you about '" + task.Title + "' — " + reminder + ".";
            }
            return "Could not set reminder. Task not found.";
        }

        public CyberTask GetLatestTask()
        {
            List<CyberTask> tasks = _storage.LoadTasks();
            if (tasks.Count == 0) return null;
            return tasks.OrderByDescending(t => t.Id).FirstOrDefault();
        }

        public bool HasIncompleteTasks()
        {
            List<CyberTask> tasks = _storage.LoadTasks();
            return tasks.Any(t => !t.IsComplete);
        }

        public string GetTaskSummary()
        {
            List<CyberTask> tasks = _storage.LoadTasks();
            int total = tasks.Count;
            int incomplete = tasks.Count(t => !t.IsComplete);
            int complete = tasks.Count(t => t.IsComplete);
            return "You have " + total + " task(s): " + incomplete + " pending, " + complete + " completed.";
        }
    }
}