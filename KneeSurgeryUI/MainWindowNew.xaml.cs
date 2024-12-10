using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

namespace KneeSurgeryUI
{
    /// <summary>
    /// Interaction logic for MainWindowNew.xaml
    /// </summary>
    public partial class MainWindowNew : Window
    {
        private const string CurrentVersion = "1.0.10.3";
        private Settings _settings = Settings.GetSettings();
        private ObservableCollection<string> _scripts = new ObservableCollection<string>();
        private ObservableCollection<string> _themes = new ObservableCollection<string>();
        private string _scriptsFolder = String.Empty;
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

            ScriptList.ItemsSource = _scripts;
            ThemeList.ItemsSource = _themes;

            GetFiles(_settings.ExplorerPath ?? AppDomain.CurrentDomain.BaseDirectory);
            GetThemes();

            ThemeList.SelectedItem = _settings.SelectedTheme.Name;

            _ = CheckForUpdatesAsync();
        }

        private void InitializeFileWatcher()
        {
            try
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
            catch
            {
                Logs.Visibility = Visibility.Hidden;
            }
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

        protected async override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (_fileWatcher != null)
            {
                _fileWatcher.EnableRaisingEvents = false;
                _fileWatcher.Dispose();
            }

            if (_settings != null)
            {
                _settings.MonacoEditorText = JsonSerializer.Deserialize<string>(await MonacoWebView.ExecuteScriptAsync("window.editor.getValue();"));
                _settings.SetSettings();
            }
        }

        private async void InitializeAsync()
        {
            await MonacoWebView.EnsureCoreWebView2Async(null);
            string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MonacoEditor.html");
            MonacoWebView.CoreWebView2.Navigate(new Uri(htmlPath).AbsoluteUri);
        }

        private async void Startup(object sender, RoutedEventArgs e)
        {
            int result = KneeSurgeryDll.KneeSurgery.Startup();

            if (result == 1)
            {
                MessageBox.Show("SirHurt has been successfully injected.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                await InjectionState();
            }
            else if (result == -1)
            {
                MessageBox.Show("Error injecting SirHurt.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show($"Unexpected result code: {result}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task InjectionState()
        {
            State.Background = Brushes.Green;

            int result = await Task.Run(() => KneeSurgeryDll.KneeSurgery.GetInjectionState());

            if (result == 1)
            {
                State.Background = Brushes.Red;
            }
            else
            {
                State.Background = Brushes.Red;
            }
        }

        private async void Execution(object sender, RoutedEventArgs e)
        {
            int result = KneeSurgeryDll.KneeSurgery.Execution(JsonSerializer.Deserialize<string>(await MonacoWebView.ExecuteScriptAsync("window.editor.getValue();")));
            _settings.MonacoEditorText = JsonSerializer.Deserialize<string>(await MonacoWebView.ExecuteScriptAsync("window.editor.getValue();"));
            _settings.SetSettings();
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

        private async void Save(object sender, RoutedEventArgs e)
        {
            string text = JsonSerializer.Deserialize<string>(await MonacoWebView.ExecuteScriptAsync("window.editor.getValue();"));

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Save",
                Filter = "Lua Files (*.lua)|*.lua",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                await File.WriteAllTextAsync(saveFileDialog.FileName, text);

                ScriptList.SelectedItem = null;
                Title = $"KneeSurgeryUI - {saveFileDialog.FileName}";
            }
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

        private void Themes(object sender, RoutedEventArgs e)
        {
            if (ThemeList.Visibility == Visibility.Visible)
            {
                ThemeList.Visibility = Visibility.Hidden;
                ThemesButton.Content = "Themes";
            }
            else
            {
                ThemeList.Visibility = Visibility.Visible;
                ThemesButton.Content = "Explorer";
            }
        }

        private async void GetSelectedFile(object sender, RoutedEventArgs e)
        {
            if (ScriptList.SelectedItem != null)
            {
                string path = Path.Combine(_scriptsFolder, ScriptList.SelectedItem.ToString());

                string fileContent = File.ReadAllText(path, System.Text.Encoding.UTF8);

                await MonacoWebView.ExecuteScriptAsync($"window.editor.setValue({JsonSerializer.Serialize(fileContent)})");

                Title = $"KneeSurgeryUI - {path}";
            }
        }

        private void GetSelectedTheme(object sender, RoutedEventArgs e)
        {
            if (ThemeList.SelectedItem != null)
            {
                Theme selectedTheme = _settings.Themes.FirstOrDefault(x => x.Name == ThemeList.SelectedItem.ToString());

                MainGrid.Background = selectedTheme.MainBackground;
                ClearButton.Foreground = selectedTheme.Foreground;
                ClearButton.Background = selectedTheme.Background;
                SaveButton.Foreground = selectedTheme.Foreground;
                SaveButton.Background = selectedTheme.Background;
                OpenFileButton.Foreground = selectedTheme.Foreground;
                OpenFileButton.Background = selectedTheme.Background;
                OpenFolderButton.Foreground = selectedTheme.Foreground;
                OpenFolderButton.Background = selectedTheme.Background;
                ThemesButton.Foreground = selectedTheme.Foreground;
                ThemesButton.Background = selectedTheme.Background;
                ScriptList.Foreground = selectedTheme.Foreground;
                ScriptList.Background = selectedTheme.Background;
                ThemeList.Foreground = selectedTheme.Foreground;
                ThemeList.Background = selectedTheme.Background;
                Logs.Foreground = selectedTheme.Foreground;
                Logs.Background = selectedTheme.Background;
                InjectButton.Foreground = selectedTheme.Foreground;
                InjectButton.Background = selectedTheme.Background;
                ExecuteButton.Foreground = selectedTheme.Foreground;
                ExecuteButton.Background = selectedTheme.Background;
                OpenLogsFolderButton.Foreground = selectedTheme.Foreground;
                OpenLogsFolderButton.Background = selectedTheme.Background;
                OpenAutoexecFolderButton.Foreground = selectedTheme.Foreground;
                OpenAutoexecFolderButton.Background = selectedTheme.Background;
                KillRobloxButton.Foreground = selectedTheme.Foreground;
                KillRobloxButton.Background = selectedTheme.Background;
                CleanRobloxButton.Foreground = selectedTheme.Foreground;
                CleanRobloxButton.Background = selectedTheme.Background;

                _settings.SelectedTheme = selectedTheme;
                _settings.SetSettings();
            }
        }

        private void GetFiles(string path)
        {
            _scripts.Clear();

            foreach (string file in Directory.GetFiles(path, "*lua", SearchOption.TopDirectoryOnly))
            {
                _scripts.Add(Path.GetFileName(file));
            }

            _scriptsFolder = path;
            _settings.ExplorerPath = path;

            _settings.SetSettings();
        }

        private void GetThemes()
        {
            _themes.Clear();

            foreach (Theme theme in _settings.Themes)
            {
                _themes.Add(theme.Name);
            }
        }

        private async Task CheckForUpdatesAsync()
        {
            string latestVersion = await GetLatestVersionAsync();

            if (Version.TryParse(CurrentVersion, out Version current) && Version.TryParse(latestVersion, out Version latest))
            {
                if (latest > current)
                {
                    MessageBoxResult result = MessageBox.Show(
                        $"A new version ({latestVersion}) is available. Would you like to download it?",
                        "Update Available",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        string releaseUrl = $"https://github.com/gapunitec/KneeSurgery/releases/tag/{latestVersion}";

                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = releaseUrl,
                            UseShellExecute = true
                        });
                    }
                }
                else
                {
                    MessageBox.Show("The application is up to date.", "No Update Available", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                if (_settings.MonacoEditorText != null)
                {
                    await MonacoWebView.ExecuteScriptAsync($"window.editor.setValue({JsonSerializer.Serialize($"{_settings.MonacoEditorText}")})");
                }
                else
                {
                    _settings.MonacoEditorText = JsonSerializer.Deserialize<string>(await MonacoWebView.ExecuteScriptAsync("window.editor.getValue();"));

                    _settings.SetSettings();
                }

                Logs.ScrollToEnd();
            }
            else
            {
                MessageBox.Show("Unable to verify the application version.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async Task<string> GetLatestVersionAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://api.github.com/repos/gapunitec/KneeSurgery/releases/latest";

                    client.DefaultRequestHeaders.Add("User-Agent", "C# App");

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    response.EnsureSuccessStatusCode();

                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                    {
                        JsonElement root = doc.RootElement;
                        string latestVersion = root.GetProperty("tag_name").GetString();

                        latestVersion = latestVersion.TrimStart('v', 'V');

                        return latestVersion;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching version: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return CurrentVersion;
            }
        }
    }
}