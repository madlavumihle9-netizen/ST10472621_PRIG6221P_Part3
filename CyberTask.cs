using System;


namespace MizzBox
{
    public class CyberTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Reminder { get; set; }
        public bool IsComplete { get; set; }
        public string CreatedAt { get; set; }

        public CyberTask()
        {
            // Default constructor for JSON deserialization
        }

        public CyberTask(int id, string title, string description, string reminder)
        {
            Id = id;
            Title = title;
            Description = description;
            Reminder = reminder;
            IsComplete = false;
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }
    }
}
