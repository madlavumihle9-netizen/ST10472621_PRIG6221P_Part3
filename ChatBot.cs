// ChatBot.cs
// The central brain of the chatbot - routes all input through features
// ProcessInput() is the single entry point called by MainWindow.xaml.cs
// This class coordinates: Memory, Sentiment, Keywords, Follow-ups, Fallbacks, NLP, Activity Log

using MizzBox;
using System;

namespace CybersecurityChatbot
{
    public class ChatBot
    {
        // Feature classes - injected via constructor for loose coupling
        private KeywordResponder _keywordResponder;
        private SentimentDetector _sentimentDetector;
        private MemoryStore _memoryStore;

        // Conversation state tracking
        private bool _awaitingName = true;
        private string _lastTopic = "";
        private string _userName = "User";
        private Random _random = new Random();

        // Part 3 additions
        private TaskManager _taskManager;
        private ActivityLogger _activityLogger;
        private bool _awaitingReminder = false;
        private int _lastTaskId = -1;

        // Constructor
        public ChatBot()
        {
            _keywordResponder = new KeywordResponder();
            _sentimentDetector = new SentimentDetector();
            _memoryStore = new MemoryStore();
            _activityLogger = new ActivityLogger();
            _taskManager = new TaskManager(new TaskStorageHelper(), _activityLogger);

            _awaitingName = true;
        }

        /// <summary>
        /// Store the user's name in memory.
        /// </summary>
        public void SetUserName(string name)
        {
            _userName = name;
            _memoryStore.UserName = name;
            _awaitingName = false;
        }

        // Returns the opening greeting
        public string GetGreeting()
        {
            return $"Welcome, {_userName}! I am your Cybersecurity Awareness Assistant. How can I help you stay safe online today?";
        }

        // MAIN ROUTING METHOD - called by MainWindow.xaml.cs for every user input
        // Part 3: NLP intent detection added as Step 2 (before follow-ups, sentiment, keywords)
        public string ProcessInput(string input)
        {
            string lowerInput = input.ToLower().Trim();

            // STEP 1: If we don't have a name yet, capture it
            if (_awaitingName)
            {
                return CaptureName(input);
            }

            // STEP 2: NLP INTENT DETECTION (Part 3 - highest priority)
            // Check for task, reminder, quiz, and log intents before falling through to Part 2 logic
            string intentResponse = DetectIntent(lowerInput, input);
            if (intentResponse != null)
            {
                _activityLogger.Log("NLP recognised intent from: '" + input + "'");
                return intentResponse;
            }

            // STEP 3: Check for follow-up phrases ("tell me more", etc.)
            if (IsFollowUpQuestion(lowerInput))
            {
                return GetFollowUpResponse();
            }

            // STEP 4: Check for memory storage patterns ("I'm interested in...")
            string memoryResponse = TryStoreMemory(input);
            if (memoryResponse != null)
            {
                return memoryResponse;
            }

            // STEP 5: Detect sentiment and get empathetic opener
            Sentiment sentiment = _sentimentDetector.Detect(input);
            string sentimentOpener = _sentimentDetector.GetSentimentResponse(sentiment);

            // STEP 6: Get keyword-based cybersecurity response
            string keywordResponse = _keywordResponder.GetResponse(lowerInput);

            if (keywordResponse != null)
            {
                // Remember this topic for follow-ups
                _lastTopic = _keywordResponder.LastMatchedKeyword;
                _activityLogger.Log("Keyword matched: " + _lastTopic + " - response delivered");

                // If sentiment detected, prepend empathetic opener + auto-share tip
                if (!string.IsNullOrEmpty(sentimentOpener))
                {
                    return sentimentOpener + "\n\n" + keywordResponse + "\n\nI hope that helps! Let me know if you want to know more.";
                }

                // Add personalised touch if we know their favourite topic
                string personalTouch = GetPersonalisedTouch();
                if (!string.IsNullOrEmpty(personalTouch))
                {
                    return personalTouch + "\n\n" + keywordResponse;
                }

                return keywordResponse;
            }

            // STEP 7: Handle special conversational phrases
            string specialResponse = HandleSpecialPhrases(lowerInput);
            if (specialResponse != null)
            {
                return specialResponse;
            }

            // STEP 8: Fallback response for unrecognised input
            return GetFallbackResponse();
        }

        // ==================== PART 3: NLP INTENT ROUTING ====================

        // NLP INTENT DETECTION (Part 3) - Enhanced with KeywordResponder for advanced detection
        // Checks for task, reminder, quiz, log, and help intents BEFORE falling through to Part 2 logic
        private string DetectIntent(string lowerInput, string originalInput)
        {
            // INTENT 1: Add Task - uses KeywordResponder for 8+ phrase variations
            string taskTitle = _keywordResponder.DetectAddTaskIntent(originalInput);
            if (taskTitle != null)
            {
                return HandleAddTaskIntent(originalInput);
            }

            // INTENT 2: Set Reminder - uses KeywordResponder for 6+ phrase variations
            if (_keywordResponder.DetectReminderIntent(originalInput))
            {
                return HandleReminderIntent(originalInput);
            }

            // INTENT 3: Start Quiz - uses KeywordResponder for 8+ phrase variations
            if (_keywordResponder.DetectQuizIntent(originalInput))
            {
                _activityLogger.Log("Quiz started via chat");
                return "🎯 Starting the Cybersecurity Quiz! Switch to the Quiz tab to begin, or type 'quiz' again to start here.";
            }

            // INTENT 4: Show Activity Log - uses KeywordResponder for 7+ phrase variations
            if (_keywordResponder.DetectLogIntent(originalInput))
            {
                return GetActivityLogResponse();
            }

            // INTENT 5: Help - uses KeywordResponder
            if (_keywordResponder.DetectHelpIntent(originalInput))
            {
                return _keywordResponder.GetHelpResponse();
            }

            return null; // No intent detected - fall through to Part 2 logic
        }

        private string HandleAddTaskIntent(string input)
        {
            // Use KeywordResponder's intelligent extraction (10+ cybersecurity topics mapped)
            string title = _keywordResponder.ExtractTaskTitleFromInput(input);
            if (string.IsNullOrWhiteSpace(title))
            {
                title = "Cybersecurity Task";
            }

            string description = "Cybersecurity task added via chatbot";
            string result = _taskManager.AddTask(title, description, "");

            // Get the task ID for reminder follow-up
            CyberTask latest = _taskManager.GetLatestTask();
            if (latest != null)
            {
                _lastTaskId = latest.Id;
                _awaitingReminder = true;
            }

            return result;
        }

        private string HandleReminderIntent(string input)
        {
            // Use KeywordResponder's intelligent time extraction
            string reminder = _keywordResponder.ExtractReminderTime(input);

            // If we have a pending task, attach reminder to it
            if (_lastTaskId > 0)
            {
                return _taskManager.UpdateReminder(_lastTaskId, reminder);
            }

            // Otherwise just acknowledge
            return "Reminder noted: " + reminder;
        }

        private string HandleReminderResponse(string input)
        {
            string lower = input.ToLower();
            if (lower.Contains("yes") || lower.Contains("sure") || lower.Contains("ok") ||
                lower.Contains("please") || lower.Contains("definitely"))
            {
                return "What reminder would you like to set? (e.g., 'Remind me in 3 days', 'tomorrow', 'next week')";
            }
            if (lower.Contains("no") || lower.Contains("nope") || lower.Contains("not now"))
            {
                _awaitingReminder = false;
                return "No reminder set. Your task is ready in the Task Assistant tab!";
            }

            // User might have directly given the reminder time
            return _taskManager.UpdateReminder(_lastTaskId, input);
        }

        private string GetActivityLogResponse()
        {
            string log = _activityLogger.GetRecentLog(10);
            if (string.IsNullOrWhiteSpace(log))
            {
                return "No activity recorded yet. Try adding a task, taking the quiz, or asking about cybersecurity topics!";
            }

            // Check if we need "show more" hint when >10 entries
            int totalCount = _activityLogger.GetCount();
            if (totalCount > 10)
            {
                return "Here's a summary of recent actions:\n\n" + log +
                       "\n\n(Type 'show more' or check the Activity Log tab to see all " + totalCount + " entries)";
            }

            return "Here's a summary of recent actions:\n\n" + log;
        }

        // STEP 1: Captures and stores the user's name
        private string CaptureName(string input)
        {
            string name = input.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                return "I didn't catch that. Could you please tell me your name?";
            }

            _memoryStore.UserName = name;
            _userName = name;
            _awaitingName = false;

            return $"Nice to meet you, {name}! I'm your Cybersecurity Awareness Assistant.\n\n" +
                   $"You can ask me about: password safety, phishing, privacy, scams, malware, " +
                   $"social engineering, two-factor authentication, Wi-Fi safety, or data backup.\n\n" +
                   $"What would you like to learn about?";
        }

        // STEP 3: Check for follow-up phrases
        private bool IsFollowUpQuestion(string input)
        {
            if (string.IsNullOrEmpty(_lastTopic))
                return false;

            string[] followUpPhrases = {
                "tell me more", "explain more", "more info", "elaborate",
                "what else", "anything else", "more", "give me another tip",
                "another one", "continue", "go on"
            };

            foreach (string phrase in followUpPhrases)
            {
                if (input.Contains(phrase))
                    return true;
            }

            return false;
        }

        private string GetFollowUpResponse()
        {
            return _keywordResponder.GetFollowUpResponse(_lastTopic);
        }

        // STEP 4: Memory storage patterns
        private string TryStoreMemory(string input)
        {
            string lowerInput = input.ToLower();

            if (lowerInput.Contains("interested in") || lowerInput.Contains("like"))
            {
                string topic = _keywordResponder.ExtractTopicFromInput(lowerInput);
                if (topic != null)
                {
                    _memoryStore.FavouriteTopic = topic;
                    return $"Great, {_userName}! I'll remember that you're interested in {topic}. " +
                           $"It's a crucial part of staying safe online. Ask me anything about it!";
                }
            }

            if (lowerInput.Contains("favourite") || lowerInput.Contains("favorite"))
            {
                string topic = _keywordResponder.ExtractTopicFromInput(lowerInput);
                if (topic != null)
                {
                    _memoryStore.FavouriteTopic = topic;
                    return $"Noted, {_userName}! I'll remember that {topic} is your favourite topic. " +
                           $"I'll make sure to share relevant tips about it during our conversation.";
                }
            }

            return null;
        }

        private string GetPersonalisedTouch()
        {
            string favouriteTopic = _memoryStore.FavouriteTopic;

            if (!string.IsNullOrEmpty(favouriteTopic) && _random.Next(3) == 0)
            {
                return $"As someone interested in {favouriteTopic}, {_userName}, here's something relevant:";
            }

            if (!string.IsNullOrEmpty(_userName) && _random.Next(5) == 0)
            {
                return $"Hey {_userName}, here's a tip for you:";
            }

            return null;
        }

        // STEP 7: Special conversational phrases
        private string HandleSpecialPhrases(string input)
        {
            if (input.Contains("how are you") || input.Contains("how r u"))
            {
                return $"I'm doing great, {_userName}! All my security systems are operational. Ready to help you stay safe online!";
            }

            if (input.Contains("purpose") || input.Contains("what do you do") || input.Contains("who are you"))
            {
                return "I am a Cybersecurity Awareness Bot. I educate South African citizens about online threats like phishing, malware, and social engineering. How can I help you today?";
            }

            if (input.Contains("help") || input.Contains("what can i ask"))
            {
                return "You can ask me about: password safety, phishing, privacy, scams, malware, social engineering, two-factor authentication, Wi-Fi safety, or data backup. You can also say 'add task', 'start quiz', or 'show activity log'. What interests you?";
            }

            if (input.Contains("bye") || input.Contains("exit") || input.Contains("quit"))
            {
                return $"Goodbye, {_userName}! Remember to stay vigilant online. Stay safe!";
            }

            return null;
        }

        // STEP 8: Fallback response
        private string GetFallbackResponse()
        {
            string[] fallbacks = {
                "I'm not sure I understand. Could you try rephrasing? You can ask about passwords, phishing, privacy, scams, malware, and more. Or try 'add task', 'start quiz', or 'show activity log'.",
                "I didn't catch that. Try asking about a cybersecurity topic like 'password safety' or 'phishing tips'. You can also manage tasks or take a quiz!",
                "Hmm, I'm not familiar with that. I can help with: passwords, phishing, malware, privacy, scams, 2FA, Wi-Fi safety, backups, tasks, quizzes, and activity logs. What would you like to know?",
                "I specialise in cybersecurity topics. Ask me about staying safe online, or try commands like 'add task', 'start quiz', or 'show activity log'!"
            };

            return fallbacks[_random.Next(fallbacks.Length)];
        }

        // Public accessors for MainWindow
        public ActivityLogger GetActivityLogger()
        {
            return _activityLogger;
        }

        public TaskManager GetTaskManager()
        {
            return _taskManager;
        }
    }
}