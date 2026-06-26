using System;
using System.Collections.Generic;
using System.Linq;


namespace MizzBox
{
    public class ActivityLogger
    {
        private List<string> _log;

        public ActivityLogger()
        {
            _log = new List<string>();
        }

        /// <summary>
        /// Log an action with a timestamp.
        /// </summary>
        public void Log(string action)
        {
            string entry = DateTime.Now.ToString("[HH:mm] ") + action;
            _log.Add(entry);
        }

        /// <summary>
        /// Get the last 'count' entries as a numbered list.
        /// </summary>
        public string GetRecentLog(int count = 10)
        {
            if (_log.Count == 0)
                return string.Empty;

            var recent = _log.Skip(Math.Max(0, _log.Count - count)).ToList();
            return FormatNumberedList(recent);
        }

        /// <summary>
        /// Get all entries as a numbered list.
        /// </summary>
        public string GetFullLog()
        {
            if (_log.Count == 0)
                return string.Empty;

            return FormatNumberedList(_log);
        }

        /// <summary>
        /// Get total number of log entries.
        /// </summary>
        public int GetCount()
        {
            return _log.Count;
        }

        /// <summary>
        /// Clear all log entries.
        /// </summary>
        public void Clear()
        {
            _log.Clear();
        }

        
        /// <summary>
        /// Format a list with numbers.
        /// </summary>
        private string FormatNumberedList(List<string> entries)
        {
            var numbered = new List<string>();
            for (int i = 0; i < entries.Count; i++)
            {
                numbered.Add($"{i + 1}. {entries[i]}");
            }
            return string.Join("\n", numbered);
        }
    }
}
