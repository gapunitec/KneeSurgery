using System.IO;
using System.Windows.Media;
using Newtonsoft.Json;

namespace KneeSurgeryUI
{
    public class Settings
    {
        public string ExplorerPath { get; set; }
        public string MonacoEditorText { get; set; }
        public bool AutoInjection { get; set; }
        public Theme SelectedTheme { get; set; }
        public List<Theme> Themes { get; set; }

        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter> { new BrushJsonConverter() }
        };

        public static Settings GetSettings()
        {
            return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json")), JsonSettings);
        }

        public void SetSettings()
        {
            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json"), JsonConvert.SerializeObject(this, JsonSettings));
        }
    }

    public class Theme
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Brush Foreground { get; set; }
        public Brush Background { get; set; }
        public Brush MainBackground { get; set; }
    }
}