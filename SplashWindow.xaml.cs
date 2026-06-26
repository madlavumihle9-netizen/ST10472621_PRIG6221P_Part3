using System;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace CybersecurityChatbot
{
    public partial class SplashWindow : Window
    {
        private SoundPlayer _voiceGreeting;
        private string _asciiArt = @"
    ██████╗██╗   ██╗██████╗ ███████╗██████╗ 
    ██╔════╝██║   ██║██╔══██╗██╔════╝██╔══██╗
    ██║     ██║   ██║██████╔╝█████╗  ██████╔╝
    ██║     ██║   ██║██╔══██╗██╔══╝  ██╔══██╗
    ╚██████╗╚██████╔╝██║  ██║███████╗██║  ██║
     ╚═════╝ ╚═════╝ ╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝
        SECURE • AWARE • PROTECTED
        ";

        public string UserName { get; private set; }

        public SplashWindow()
        {
            InitializeComponent();
            Loaded += SplashWindow_Loaded;
        }

        private async void SplashWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SplashAsciiArt.Text = _asciiArt;

            // Start glow animation
            if (FindResource("GlowAnimation") is Storyboard glow)
            {
                glow.Begin(TitleGlow);
            }

            // Simulate initialization sequence
            await UpdateStatus("Initializing secure connection...", 500);
            await UpdateStatus("Loading cybersecurity modules...", 800);
            await UpdateStatus("Calibrating threat detection...", 600);
            await UpdateStatus("Ready. Please identify yourself.", 400);

            // Fade in the input section
            if (FindResource("SlideUp") is Storyboard slideUp)
            {
                slideUp.Begin(InputSection);
            }

            // Play voice greeting
            PlayVoiceGreeting();

            txtSplashName.Focus();
        }

        private async Task UpdateStatus(string message, int delayMs)
        {
            txtStatus.Text = ">>> " + message;
            await Task.Delay(delayMs);
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                _voiceGreeting = new SoundPlayer("greeting.wav");
                _voiceGreeting.Play();
            }
            catch (Exception ex)
            {
                txtStatus.Text = ">>> Audio unavailable: " + ex.Message;
            }
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            ProcessNameEntry();
        }

        private void txtSplashName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessNameEntry();
            }
        }

        private void ProcessNameEntry()
        {
            string name = txtSplashName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                txtStatus.Text = ">>> ERROR: Name required for authentication.";
                txtStatus.Foreground = System.Windows.Media.Brushes.Red;
                txtSplashName.Focus();
                return;
            }

            UserName = name;
            txtStatus.Text = ">>> Welcome, " + name + ". Access granted.";
            txtStatus.Foreground = System.Windows.Media.Brushes.Green;

            // Set DialogResult to true so App.xaml.cs knows user entered a name
            this.DialogResult = true;
            this.Close();
        }
    }
}