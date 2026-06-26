using System.Windows;

namespace CybersecurityChatbot
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create main window first but DON'T show it yet
            var mainWindow = new MainWindow();
            this.MainWindow = mainWindow;  // Set as main window so app stays alive

            // Show splash screen as dialog
            var splash = new SplashWindow();
            bool? result = splash.ShowDialog();

            // After splash closes, check if name was entered
            if (result == true && !string.IsNullOrWhiteSpace(splash.UserName))
            {
                // Set the name and show the main window
                mainWindow.SetUserName(splash.UserName);
                mainWindow.Show();
            }
            else
            {
                // User closed without entering name
                Shutdown();
            }
        }
    }
}