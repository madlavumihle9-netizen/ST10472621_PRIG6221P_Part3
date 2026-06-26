using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MizzBox;

namespace CybersecurityChatbot
{
    public partial class MainWindow : Window
    {
        private ChatBot _chatBot;
        private TaskManager _taskManager;
        private QuizManager _quizManager;
        private ActivityLogger _activityLogger;

        private void UpdateLogButtonLabel()
        {
            int count = _activityLogger.GetCount();
            btnShowRecentLog.Content = $"📋 Recent ({Math.Min(count, 10)})";
        }

        public MainWindow()
        {
            InitializeComponent();
            LoadAsciiArt();
            _chatBot = new ChatBot();
            _activityLogger = _chatBot.GetActivityLogger();
            _taskManager = _chatBot.GetTaskManager();
            _quizManager = new QuizManager();

            // Don't show greeting here — splash screen handles welcome
            // The SetUserName() call will show the personalized welcome

            UserInputTextBox.GotFocus += (s, e) =>
            {
                if (UserInputTextBox.Text == "Type your message here...")
                    UserInputTextBox.Text = "";
            };

            UserInputTextBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(UserInputTextBox.Text))
                    UserInputTextBox.Text = "Type your message here...";
            };

            // Load tasks into DataGrid on startup
            RefreshTaskList();
        }

        // ==================== SPLASH SCREEN ====================
        public void SetUserName(string name)
        {
            // Store name in chatbot memory
            _chatBot.SetUserName(name);
            AppendBotMessage($"Welcome, {name}! Your cybersecurity session is now active.\n\nType 'help' to see what I can do, or explore the Task Assistant and Quiz tabs.");
        }

        // ==================== EXISTING CHAT METHODS ====================
        private void LoadAsciiArt()
        {
            string asciiArt = @"
   ____      _                  _                  _   
  / ___|   _| |__   ___  _ __  | |__   ___  _ __  | |_ 
 | |  | | | | '_ \ / _ \| '__| | '_ \ / _ \| '__| | __|
 | |__| |_| | |_) | (_) | |    | |_) | (_) | |    | |_ 
  \____\__, |_.__/ \___/|_|    |_.__/ \___/|_|     \__|
       |___/                                            
";
            AsciiArtTextBlock.Text = asciiArt;
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = "-windowstyle hidden -c \"(New-Object Media.SoundPlayer 'greeting.wav').PlaySync()\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch
            {
                // Silent fail
            }
        }

        private void PlayVoiceGoodbye()
        {
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = "-windowstyle hidden -c \"(New-Object Media.SoundPlayer 'goodbye.wav').PlaySync()\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch
            {
                // Silent fail
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void UserInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

        private void SendMessage()
        {
            string input = UserInputTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(input) || input == "Type your message here...")
                return;

            AppendUserMessage(input);
            UserInputTextBox.Text = "";

            string response = _chatBot.ProcessInput(input);

            if (response.Contains("Goodbye"))
            {
                PlayVoiceGoodbye();
            }

            AppendBotMessage(response);
            ChatScrollViewer.ScrollToBottom();
        }

        private void AppendUserMessage(string message)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(26, 31, 75)),
                CornerRadius = new CornerRadius(10, 10, 0, 10),
                Padding = new Thickness(12),
                Margin = new Thickness(50, 5, 5, 5),
                HorizontalAlignment = HorizontalAlignment.Right,
                MaxWidth = 600
            };

            var textBlock = new TextBlock
            {
                Text = message,
                Foreground = Brushes.White,
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap
            };

            border.Child = textBlock;
            ChatStackPanel.Children.Add(border);
        }

        private void AppendBotMessage(string message)
        {
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(13, 18, 53)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(0, 212, 170)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(10, 10, 10, 0),
                Padding = new Thickness(12),
                Margin = new Thickness(5, 5, 50, 5),
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxWidth = 600
            };

            var textBlock = new TextBlock
            {
                Text = message,
                Foreground = new SolidColorBrush(Color.FromRgb(0, 212, 170)),
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                LineHeight = 20
            };

            border.Child = textBlock;
            ChatStackPanel.Children.Add(border);
        }

        // ==================== TASK ASSISTANT HANDLERS ====================
        private void btnAddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = txtTaskTitle.Text.Trim();
            string description = txtTaskDescription.Text.Trim();
            string reminder = txtTaskReminder.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter a task title.", "Missing Title", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (reminder == "e.g., Remind me in 3 days")
                reminder = "";

            string result = _taskManager.AddTask(title, description, reminder);
            AppendBotMessage(result);
            RefreshTaskList();

            // Clear inputs
            txtTaskTitle.Text = "";
            txtTaskDescription.Text = "";
            txtTaskReminder.Text = "e.g., Remind me in 3 days";
        }

        private void btnMarkComplete_Click(object sender, RoutedEventArgs e)
        {
            if (dgTasks.SelectedItem is CyberTask selectedTask)
            {
                string result = _taskManager.MarkAsComplete(selectedTask.Id);
                AppendBotMessage(result);
                RefreshTaskList();
            }
            else
            {
                MessageBox.Show("Please select a task to mark as complete.", "No Task Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (dgTasks.SelectedItem is CyberTask selectedTask)
            {
                var result = MessageBox.Show($"Delete task '{selectedTask.Title}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    string msg = _taskManager.DeleteTask(selectedTask.Id);
                    AppendBotMessage(msg);
                    RefreshTaskList();
                }
            }
            else
            {
                MessageBox.Show("Please select a task to delete.", "No Task Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnRefreshTasks_Click(object sender, RoutedEventArgs e)
        {
            RefreshTaskList();
            AppendBotMessage(_taskManager.GetTaskSummary());
        }

        private void RefreshTaskList()
        {
            List<CyberTask> tasks = _taskManager.GetAllTasks();
            dgTasks.ItemsSource = tasks;
            dgTasks.Items.Refresh();

            if (tasks.Count == 0)
            {
                txtTaskSummary.Text = "No tasks yet. Add your first cybersecurity task above!";
            }
            else
            {
                int incomplete = tasks.Count(t => !t.IsComplete);
                txtTaskSummary.Text = $"You have {tasks.Count} task(s): {incomplete} pending, {tasks.Count - incomplete} completed.";
            }
        }

        // ==================== QUIZ HANDLERS ====================
        private void btnStartQuiz_Click(object sender, RoutedEventArgs e)
        {
            _quizManager.ResetQuiz();
            _activityLogger.Log("Quiz started");

            btnStartQuiz.Visibility = Visibility.Collapsed;
            btnSubmitAnswer.Visibility = Visibility.Visible;
            btnNextQuestion.Visibility = Visibility.Collapsed;
            btnRestartQuiz.Visibility = Visibility.Collapsed;
            borderFeedback.Visibility = Visibility.Collapsed;

            DisplayCurrentQuestion();
            UpdateQuizScore();
        }

        private void btnSubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            string selectedAnswer = GetSelectedQuizAnswer();
            if (string.IsNullOrEmpty(selectedAnswer))
            {
                MessageBox.Show("Please select an answer.", "No Answer", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            bool isCorrect = _quizManager.SubmitAnswer(selectedAnswer);
            string feedback = _quizManager.GetFeedback(isCorrect);

            txtFeedback.Text = feedback;
            txtFeedback.Foreground = isCorrect
                ? new SolidColorBrush(Color.FromRgb(0, 212, 170))
                : new SolidColorBrush(Color.FromRgb(220, 53, 69));

            borderFeedback.Visibility = Visibility.Visible;
            btnSubmitAnswer.Visibility = Visibility.Collapsed;
            btnNextQuestion.Visibility = Visibility.Visible;

            if (_quizManager.IsFinished())
            {
                btnNextQuestion.Visibility = Visibility.Collapsed;
                btnRestartQuiz.Visibility = Visibility.Visible;
                ShowFinalQuizResults();
            }

            UpdateQuizScore();
        }

        private void btnNextQuestion_Click(object sender, RoutedEventArgs e)
        {
            btnSubmitAnswer.Visibility = Visibility.Visible;
            btnNextQuestion.Visibility = Visibility.Collapsed;
            borderFeedback.Visibility = Visibility.Collapsed;

            DisplayCurrentQuestion();
        }

        private void btnRestartQuiz_Click(object sender, RoutedEventArgs e)
        {
            btnStartQuiz_Click(sender, e);
        }

        private void DisplayCurrentQuestion()
        {
            var question = _quizManager.GetCurrentQuestion();
            if (question == null) return;

            txtQuestionText.Text = question.Question;
            spQuizOptions.Children.Clear();

            if (question.IsTrueFalse)
            {
                AddQuizOption("True");
                AddQuizOption("False");
            }
            else
            {
                for (int i = 0; i < question.Options.Count; i++)
                {
                    AddQuizOption($"{(char)('A' + i)}) {question.Options[i]}");
                }
            }
        }

        private void AddQuizOption(string optionText)
        {
            var radio = new RadioButton
            {
                Content = optionText,
                Foreground = Brushes.White,
                FontSize = 14,
                Margin = new Thickness(0, 8, 0, 8),
                GroupName = "QuizOptions"
            };
            spQuizOptions.Children.Add(radio);
        }

        private string GetSelectedQuizAnswer()
        {
            foreach (var child in spQuizOptions.Children)
            {
                if (child is RadioButton radio && radio.IsChecked == true)
                {
                    string content = radio.Content.ToString();
                    // Extract just the letter for multiple choice, or True/False
                    if (content.StartsWith("A)")) return "A";
                    if (content.StartsWith("B)")) return "B";
                    if (content.StartsWith("C)")) return "C";
                    if (content.StartsWith("D)")) return "D";
                    return content; // True or False
                }
            }
            return null;
        }

        private void UpdateQuizScore()
        {
            txtQuizScore.Text = $"{_quizManager.GetCurrentScore()} / {_quizManager.GetCurrentIndex()}";
            txtQuizProgress.Text = $" | Question {_quizManager.GetCurrentIndex() + 1} of {_quizManager.GetTotalQuestions()}";
        }

        private void ShowFinalQuizResults()
        {
            string finalMessage = _quizManager.GetFinalMessage();
            int score = _quizManager.GetCurrentScore();
            int total = _quizManager.GetTotalQuestions();

            txtQuestionText.Text = $"Quiz Complete!\n\nYour Score: {score} out of {total}\n\n{finalMessage}";
            spQuizOptions.Children.Clear();
            _activityLogger.Log($"Quiz completed - Score: {score} out of {total}");
        }

        // ==================== ACTIVITY LOG HANDLERS ====================
        private void btnShowRecentLog_Click(object sender, RoutedEventArgs e)
        {
            string log = _activityLogger.GetRecentLog(10);
            txtActivityLog.Text = string.IsNullOrWhiteSpace(log)
                ? "No activity recorded yet."
                : "Here's a summary of recent actions:\n\n" + log;
        }

        private void btnShowFullLog_Click(object sender, RoutedEventArgs e)
        {
            string log = _activityLogger.GetFullLog();
            txtActivityLog.Text = string.IsNullOrWhiteSpace(log)
                ? "No activity recorded yet."
                : "Full Activity Log:\n\n" + log;
        }

        private void btnClearLog_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Clear all activity logs?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _activityLogger.Clear();
                txtActivityLog.Text = "Activity log cleared.";
            }
        }
    }
}