using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using Microsoft.Win32;

namespace KneeSurgeryUI
{
    /// <summary>
    /// Interaction logic for MainWindowNew.xaml
    /// </summary>
    public partial class MainWindowNew : Window
    {
        private ObservableCollection<string> scripts = new ObservableCollection<string>();
        private string scriptsFolder = String.Empty;

        public MainWindowNew()
        {
            InitializeComponent();
            InitializeAsync();

            ScriptList.ItemsSource = scripts;
            GetFiles(AppDomain.CurrentDomain.BaseDirectory);
        }

        private async void InitializeAsync()
        {
            await MonacoWebView.EnsureCoreWebView2Async(null);
            string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MonacoEditor.html");
            MonacoWebView.CoreWebView2.Navigate(new Uri(htmlPath).AbsoluteUri);
        }

        private void Startup(object sender, RoutedEventArgs e)
        {
            int result = KneeSurgeryDll.KneeSurgery.Startup();
        }

        private async void Execution(object sender, RoutedEventArgs e)
        {
            int result = KneeSurgeryDll.KneeSurgery.Execution(JsonSerializer.Deserialize<string>(await MonacoWebView.ExecuteScriptAsync("window.editor.getValue();")));
        }

        private void OpenLogsFolder(object sender, RoutedEventArgs e)
        {
            int result = KneeSurgeryDll.KneeSurgery.OpenLogsFolder();
        }

        private void OpenAutoexecFolder(object sender, RoutedEventArgs e)
        {
            int result = KneeSurgeryDll.KneeSurgery.OpenAutoexecFolder();
        }

        private void KillRobloxPlayerBeta(object sender, RoutedEventArgs e)
        {
            int result = KneeSurgeryDll.KneeSurgery.KillRobloxPlayerBeta();
        }

        private async void Clear(object sender, RoutedEventArgs e)
        {
            await MonacoWebView.ExecuteScriptAsync($"window.editor.setValue({JsonSerializer.Serialize("")})");

            ScriptList.SelectedItem = null;
            Title = "KneeSurgeryUI";
        }

        private async void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open file",
                Filter = "Lua Files (*.lua)|*.lua",
                Multiselect = false,
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string fileContent = File.ReadAllText(openFileDialog.FileName, System.Text.Encoding.UTF8);

                await MonacoWebView.ExecuteScriptAsync($"window.editor.setValue({JsonSerializer.Serialize(fileContent)})");

                ScriptList.SelectedItem = null;
                Title = $"KneeSurgeryUI - {openFileDialog.FileName}";
            }
        }

        private void OpenFolder(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog
            {
                Title = "Open folder",
                Multiselect = false,
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            if (openFolderDialog.ShowDialog() == true)
            {
                GetFiles(openFolderDialog.FolderName);
            }
        }

        private async void GetSelectedFile(object sender, RoutedEventArgs e)
        {
            if (ScriptList.SelectedItem != null)
            {
                string path = Path.Combine(scriptsFolder, ScriptList.SelectedItem.ToString());

                string fileContent = File.ReadAllText(path, System.Text.Encoding.UTF8);

                await MonacoWebView.ExecuteScriptAsync($"window.editor.setValue({JsonSerializer.Serialize(fileContent)})");

                Title = $"KneeSurgeryUI - {path}";
            }
        }

        private void GetFiles(string path)
        {
            scripts.Clear();

            foreach (string file in Directory.GetFiles(path, "*lua", SearchOption.TopDirectoryOnly))
            {
                scripts.Add(Path.GetFileName(file));
            }

            scriptsFolder = path;
        }
    }
}
