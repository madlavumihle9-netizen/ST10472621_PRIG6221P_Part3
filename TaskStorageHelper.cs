using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MizzBox
{
    public class TaskStorageHelper
    {
        // File path: same folder as the .exe
        private const string FilePath = "tasks.json";

        /// <summary>
        /// Load all tasks from tasks.json.
        /// If file doesn't exist, returns empty list.
        /// </summary>
        public List<CyberTask> LoadTasks()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    return new List<CyberTask>();
                }

                string json = File.ReadAllText(FilePath);
                List<CyberTask> tasks = JsonConvert.DeserializeObject<List<CyberTask>>(json);

                return tasks ?? new List<CyberTask>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tasks: {ex.Message}");
                return new List<CyberTask>();
            }
        }

        /// <summary>
        /// Save all tasks to tasks.json.
        /// Overwrites the file completely.
        /// </summary>
        public void SaveTasks(List<CyberTask> tasks)
        {
            try
            {
                string json = JsonConvert.SerializeObject(tasks, Formatting.Indented);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving tasks: {ex.Message}");
            }
        }

        /// <summary>
        /// Add a new task. Auto-generates ID (last ID + 1).
        /// </summary>
        public CyberTask AddTask(string title, string description, string reminder)
        {
            List<CyberTask> tasks = LoadTasks();

            int newId = 1;
            if (tasks.Count > 0)
            {
                newId = tasks.Max(t => t.Id) + 1;
            }

            CyberTask newTask = new CyberTask(newId, title, description, reminder);
            tasks.Add(newTask);
            SaveTasks(tasks);

            return newTask;
        }

        /// <summary>
        /// Mark a task as complete by ID.
        /// Returns true if found and updated, false otherwise.
        /// </summary>
        public bool MarkAsComplete(int id)
        {
            List<CyberTask> tasks = LoadTasks();
            CyberTask task = tasks.FirstOrDefault(t => t.Id == id);

            if (task != null)
            {
                task.IsComplete = true;
                SaveTasks(tasks);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Delete a task by ID.
        /// Returns true if found and deleted, false otherwise.
        /// </summary>
        public bool DeleteTask(int id)
        {
            List<CyberTask> tasks = LoadTasks();
            CyberTask task = tasks.FirstOrDefault(t => t.Id == id);

            if (task != null)
            {
                tasks.Remove(task);
                SaveTasks(tasks);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get a single task by ID.
        /// Returns null if not found.
        /// </summary>
        public CyberTask GetTaskById(int id)
        {
            List<CyberTask> tasks = LoadTasks();
            return tasks.FirstOrDefault(t => t.Id == id);
        }

        /// <summary>
        /// Get the highest ID currently in use.
        /// Useful for auto-incrementing.
        /// </summary>
        public int GetMaxId()
        {
            List<CyberTask> tasks = LoadTasks();
            if (tasks.Count == 0) return 0;
            return tasks.Max(t => t.Id);
        }
    }
}