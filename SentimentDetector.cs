// SentimentDetector.cs
// Detects emotional tone in user messages and returns empathetic openers
// Uses enum for type safety and Dictionary for efficient lookup
// Auto-shares tips after sentiment detection (required by rubric)

using System;
using System.Collections.Generic;

namespace CybersecurityChatbot
{
    // Enum for sentiment types - makes code readable and type-safe
    public enum Sentiment
    {
        Neutral,
        Worried,
        Curious,
        Frustrated,
        Happy
    }

    public class SentimentDetector
    {
        // Dictionary mapping each sentiment to its trigger words
        private Dictionary<Sentiment, List<string>> _sentimentTriggers;

        // Constructor: populate trigger word dictionary
        public SentimentDetector()
        {
            _sentimentTriggers = new Dictionary<Sentiment, List<string>>
            {
                [Sentiment.Worried] = new List<string>
                {
                    "worried", "scared", "afraid", "nervous", "anxious", "concerned",
                    "fear", "terrified", "panic", "stressed", "uneasy", "troubled"
                },

                [Sentiment.Curious] = new List<string>
                {
                    "curious", "wondering", "interested", "want to know", "how does",
                    "what is", "tell me about", "explain", "learn", "know more"
                },

                [Sentiment.Frustrated] = new List<string>
                {
                    "frustrated", "annoyed", "confused", "don't understand", "difficult",
                    "complicated", "hard", "struggling", "stuck", "angry", "upset", "mad"
                },

                [Sentiment.Happy] = new List<string>
                {
                    "great", "thanks", "helpful", "awesome", "love it", "amazing",
                    "excellent", "good", "happy", "pleased", "grateful", "appreciate"
                }
            };
        }

        // Analyzes input and returns detected sentiment
        // Defaults to Neutral if no trigger words found
        public Sentiment Detect(string input)
        {
            string lowerInput = input.ToLower();

            // Check each sentiment's trigger words
            foreach (var sentimentPair in _sentimentTriggers)
            {
                foreach (string trigger in sentimentPair.Value)
                {
                    if (lowerInput.Contains(trigger))
                    {
                        return sentimentPair.Key;
                    }
                }
            }

            return Sentiment.Neutral;
        }

        // Returns empathetic opening sentence based on sentiment
        // Returns empty string for Neutral so nothing is prepended
        public string GetSentimentResponse(Sentiment sentiment)
        {
            switch (sentiment)
            {
                case Sentiment.Worried:
                    return "I completely understand your concern. Cybersecurity can feel overwhelming, but you're taking the right steps by learning about it. Let me help ease your worries with some practical advice.";

                case Sentiment.Curious:
                    return "I love your curiosity! Learning about cybersecurity is one of the best investments you can make for your digital safety. Let me share some interesting insights with you.";

                case Sentiment.Frustrated:
                    return "I hear your frustration, and that's completely valid. Technology can be confusing, but don't worry - we'll break this down into simple, manageable steps together.";

                case Sentiment.Happy:
                    return "I'm so glad you're feeling positive about this! Your enthusiasm for staying safe online is wonderful. Here's some more information to keep you protected.";

                case Sentiment.Neutral:
                default:
                    return ""; // No opener for neutral - keeps response clean
            }
        }
    }
}