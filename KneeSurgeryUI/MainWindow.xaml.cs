using System.IO;
using System.Text.Json;
using System.Windows;

namespace KneeSurgeryUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await MonacoWebView.EnsureCoreWebView2Async(null);
            string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MonacoEditor.html");
            MonacoWebView.CoreWebView2.Navigate(new Uri(htmlPath).AbsoluteUri);
        }

        private void InitializeTest(object sender, RoutedEventArgs e)
        {
            int result = KneeSurgeryDll.KneeSurgery.Initialize(null);

            MessageBox.Show(result == 1 ? "Initialize.test succeeded" : "Initialize.test failed");
        }

        private void InjectionTest(object sender, RoutedEventArgs e)
        {
            int result = KneeSurgeryDll.KneeSurgery.Injection();

            MessageBox.Show(result == 1 ? "Injection.test succeeded" : "Injection.test failed");
        }

        private void AutoexecTest(object sender, RoutedEventArgs e)
        {
            int result = KneeSurgeryDll.KneeSurgery.Autoexec();

            MessageBox.Show(result == 1 ? "Autoexec.test succeeded" : "Autoexec.test failed");
        }

        private void StartupTest(object sender, RoutedEventArgs e)
        {
            int result = KneeSurgeryDll.KneeSurgery.Startup();

            MessageBox.Show(result == 1 ? "Startup.test succeeded" : "Startup.test failed");
        }

        private async void ExecutionTest(object sender, RoutedEventArgs e)
        {
            int result = KneeSurgeryDll.KneeSurgery.Execution(JsonSerializer.Deserialize<string>(await MonacoWebView.ExecuteScriptAsync("window.editor.getValue();")));

            MessageBox.Show(result == 1 ? "Execution.test succeeded" : "Execution.test failed");
        }

        private void OpenLogsFolderTest(object sender, RoutedEventArgs e)
        {
            int result = KneeSurgeryDll.KneeSurgery.OpenLogsFolder();

            MessageBox.Show(result == 1 ? "OpenLogsFolder.test succeeded" : "OpenLogsFolder.test failed");
        }

        private void OpenAutoexecFolderTest(object sender, RoutedEventArgs e)
        {
            int result = KneeSurgeryDll.KneeSurgery.OpenAutoexecFolder();

            MessageBox.Show(result == 1 ? "OpenAutoexecFolder.test succeeded" : "OpenAutoexecFolder.test failed");
        }

        private void KillRobloxPlayerBetaTest(object sender, RoutedEventArgs e)
        {
            int result = KneeSurgeryDll.KneeSurgery.KillRobloxPlayerBeta();

            MessageBox.Show(result == 1 ? "KillRobloxPlayerBeta.test succeeded" : "KillRobloxPlayerBeta.test failed");
        }
    }
}