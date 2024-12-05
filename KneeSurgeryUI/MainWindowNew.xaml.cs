using System.Collections.ObjectModel;
using System.IO;
using System.Text;
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
        private FileSystemWatcher _fileWatcher;
        private readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "sirhurt", "sirhui", "sirh_debug_log.dat");
        private readonly SynchronizationContext _syncContext;

        public MainWindowNew()
        {
            InitializeComponent();
            _syncContext = SynchronizationContext.Current;
            InitializeFileWatcher();
            LoadFileContent();
            InitializeAsync();

            ScriptList.ItemsSource = scripts;
            GetFiles(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void InitializeFileWatcher()
        {
            string directoryName = Path.GetDirectoryName(_filePath);
            string fileName = Path.GetFileName(_filePath);

            _fileWatcher = new FileSystemWatcher
            {
                Path = directoryName,
                Filter = fileName,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
            };

            _fileWatcher.Changed += OnFileChanged;
            _fileWatcher.Created += OnFileChanged;
            _fileWatcher.Deleted += OnFileDeleted;
            _fileWatcher.Renamed += OnFileRenamed;

            _fileWatcher.EnableRaisingEvents = true;
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(100);

            _syncContext.Post(_ => LoadFileContent(), null);
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            _syncContext.Post(_ =>
            {
                Logs.Text = "The file has been deleted.";
            }, null);
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            _syncContext.Post(_ =>
            {
                Logs.Text = $"The file has been renamed to {e.Name}.";
            }, null);
        }

        private void LoadFileContent()
        {
            if (!File.Exists(_filePath))
            {
                Logs.Text = "File does not exist.";

                return;
            }

            string content = ReadFileSafe(_filePath);
            Logs.Text = content;
            Logs.ScrollToEnd();
        }

        private string ReadFileSafe(string path)
        {
            const int maxRetries = 5;
            const int delay = 100;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
                catch (IOException)
                {
                    Thread.Sleep(delay);
                }
            }

            throw new IOException("Failed to read the file after multiple attempts.");
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (_fileWatcher != null)
            {
                _fileWatcher.EnableRaisingEvents = false;
                _fileWatcher.Dispose();
            }
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

            if (result == 1)
            {
                MessageBox.Show("Roblox Player has been successfully terminated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (result == -1)
            {
                MessageBox.Show("Error terminating Roblox Player.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show($"Unexpected result code: {result}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CleanRobloxPlayerBeta(object sender, RoutedEventArgs e)
        {
            int result = KneeSurgeryDll.KneeSurgery.CleanRobloxPlayerBeta();

            if (result == 1)
            {
                MessageBox.Show("Roblox Player has been successfully cleaned.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (result == -1)
            {
                MessageBox.Show("Error cleaning Roblox Player.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show($"Unexpected result code: {result}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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