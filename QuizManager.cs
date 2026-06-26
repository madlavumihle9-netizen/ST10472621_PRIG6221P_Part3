using System;
using System.Collections.Generic;

namespace MizzBox
{
    public class QuizQuestion
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public string CorrectAnswer { get; set; }
        public string Explanation { get; set; }
        public bool IsTrueFalse { get; set; }

        public QuizQuestion(string question, List<string> options, string correctAnswer, string explanation, bool isTrueFalse = false)
        {
            Question = question;
            Options = options;
            CorrectAnswer = correctAnswer;
            Explanation = explanation;
            IsTrueFalse = isTrueFalse;
        }
    }

    public class QuizManager
    {
        private List<QuizQuestion> _questions;
        private int _currentIndex = 0;
        private int _score = 0;

        public QuizManager()
        {
            _questions = new List<QuizQuestion>();
            PopulateQuestions();
        }

        /// <summary>
        /// Load 12 cybersecurity questions (mix of MCQ and True/False).
        /// </summary>
        private void PopulateQuestions()
        {
            // Phishing
            _questions.Add(new QuizQuestion(
                "What should you do if you receive an email asking for your password?",
                new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                "C",
                "Correct! Reporting phishing emails helps prevent scams and protects others.",
                false));

            _questions.Add(new QuizQuestion(
                "Phishing emails often use urgent language to pressure you into acting quickly.",
                new List<string>(),
                "True",
                "Correct! Phishing attacks rely on urgency and fear to trick victims.",
                true));

            // Password Safety
            _questions.Add(new QuizQuestion(
                "Which of the following is the strongest password?",
                new List<string> { "password123", "P@ssw0rd", "Tr0ub4dor&3", "Xk9#mP2$vLqW" },
                "D",
                "Correct! A strong password uses a mix of uppercase, lowercase, numbers, and symbols with no dictionary words.",
                false));

            _questions.Add(new QuizQuestion(
                "You should use the same password for all your accounts to make them easier to remember.",
                new List<string>(),
                "False",
                "Correct! Using the same password everywhere means one breach compromises all your accounts.",
                true));

            // Safe Browsing
            _questions.Add(new QuizQuestion(
                "What does the 'S' in HTTPS stand for?",
                new List<string> { "Safe", "Secure", "Standard", "System" },
                "B",
                "Correct! HTTPS means HyperText Transfer Protocol Secure — it encrypts your data.",
                false));

            _questions.Add(new QuizQuestion(
                "It's safe to do online banking on public Wi-Fi at a coffee shop.",
                new List<string>(),
                "False",
                "Correct! Public Wi-Fi is often unencrypted. Use a VPN or your mobile data for sensitive transactions.",
                true));

            // Social Engineering
            _questions.Add(new QuizQuestion(
                "A caller claims to be from your bank and asks for your OTP. What should you do?",
                new List<string> { "Give them the OTP", "Hang up and call your bank directly", "Ask for their employee ID", "Give them partial information" },
                "B",
                "Correct! Banks never ask for OTPs. Always hang up and call the official number.",
                false));

            _questions.Add(new QuizQuestion(
                "Social engineering attacks exploit human psychology rather than technical vulnerabilities.",
                new List<string>(),
                "True",
                "Correct! Social engineering manipulates people into breaking security procedures.",
                true));

            // 2FA
            _questions.Add(new QuizQuestion(
                "What is the main benefit of Two-Factor Authentication (2FA)?",
                new List<string> { "It makes logging in faster", "It adds an extra layer of security", "It remembers your password", "It works without internet" },
                "B",
                "Correct! 2FA requires something you know (password) AND something you have (phone/token).",
                false));

            // Malware
            _questions.Add(new QuizQuestion(
                "Ransomware encrypts your files and demands payment to unlock them.",
                new List<string>(),
                "True",
                "Correct! Ransomware is a type of malware that locks your data until you pay a ransom.",
                true));

            // Privacy Settings
            _questions.Add(new QuizQuestion(
                "Which social media privacy setting is most secure?",
                new List<string> { "Public", "Friends only", "Friends of friends", "Custom" },
                "D",
                "Correct! Custom settings let you control exactly who sees what on your profile.",
                false));

            // Data Backup
            _questions.Add(new QuizQuestion(
                "How often should you back up important data?",
                new List<string> { "Once a year", "Only when you remember", "Regularly (weekly or daily)", "Never, cloud storage is enough" },
                "C",
                "Correct! Regular backups protect you from data loss due to ransomware, hardware failure, or theft.",
                false));
        }

        // ==================== QUIZ CONTROL METHODS ====================

        public QuizQuestion GetCurrentQuestion()
        {
            if (_currentIndex >= 0 && _currentIndex < _questions.Count)
                return _questions[_currentIndex];
            return null;
        }

        public bool SubmitAnswer(string answer)
        {
            var question = GetCurrentQuestion();
            if (question == null) return false;

            bool isCorrect = answer.Equals(question.CorrectAnswer, StringComparison.OrdinalIgnoreCase);
            if (isCorrect)
                _score++;

            _currentIndex++;
            return isCorrect;
        }

        public string GetFeedback(bool correct)
        {
            var question = _questions[Math.Max(0, _currentIndex - 1)];
            if (correct)
                return "✅ Correct! " + question.Explanation;
            else
                return "❌ Incorrect. " + question.Explanation;
        }

        public bool IsFinished()
        {
            return _currentIndex >= _questions.Count;
        }

        public int GetCurrentScore()
        {
            return _score;
        }

        public int GetCurrentIndex()
        {
            return _currentIndex;
        }

        public int GetTotalQuestions()
        {
            return _questions.Count;
        }

        public string GetFinalMessage()
        {
            double percentage = (_score / (double)_questions.Count) * 100;

            if (percentage >= 90)
                return "🏆 Outstanding! You're a cybersecurity expert!";
            else if (percentage >= 70)
                return "🌟 Great job! You know your stuff!";
            else if (percentage >= 50)
                return "👍 Not bad! Keep learning to stay safe online.";
            else
                return "📚 Keep studying! Cybersecurity awareness takes practice.";
        }

        public void ResetQuiz()
        {
            _currentIndex = 0;
            _score = 0;
        }
    }
}