// MemoryStore.cs
// Stores and recalls user information throughout the conversation
// Uses properties and generic Dictionary for extensible key-value storage
// Enables personalised responses that enhance engagement

using System.Collections.Generic;

namespace CybersecurityChatbot
{
    public class MemoryStore
    {
        // Core user information - used for personalisation
        public string UserName { get; set; } = "";
        public string FavouriteTopic { get; set; } = "";

        // Extensible storage for any key-value pair
        // Allows future expansion without modifying class structure
        private Dictionary<string, string> _memory = new Dictionary<string, string>();

        // Stores any key-value pair (extensible memory)
        public void Store(string key, string value)
        {
            _memory[key] = value;
        }

        // Retrieves a stored value by key
        public string Recall(string key)
        {
            return _memory.ContainsKey(key) ? _memory[key] : null;
        }

        // Builds a personalised greeting using stored information
        public string GetPersonalisedOpener()
        {
            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(FavouriteTopic))
            {
                return $"Welcome back, {UserName}! I remember you're interested in {FavouriteTopic}. ";
            }
            else if (!string.IsNullOrEmpty(UserName))
            {
                return $"Welcome back, {UserName}! ";
            }

            return "";
        }

        // Checks if we have enough information to personalise
        public bool CanPersonalise()
        {
            return !string.IsNullOrEmpty(UserName);
        }
    }
}