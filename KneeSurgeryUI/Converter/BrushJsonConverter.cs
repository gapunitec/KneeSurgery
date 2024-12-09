using System.Windows.Media;
using Newtonsoft.Json;

public class BrushJsonConverter : JsonConverter<Brush>
{
    public override void WriteJson(JsonWriter writer, Brush value, JsonSerializer serializer)
    {
        if (value is SolidColorBrush solidColorBrush)
        {
            string colorString = solidColorBrush.Color.ToString();
            writer.WriteValue(colorString);
        }
        else
        {
            throw new NotSupportedException("Solo SolidColorBrush è supportato.");
        }
    }

    public override Brush ReadJson(JsonReader reader, Type objectType, Brush existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        string colorString = (string)reader.Value;

        if (ColorConverter.ConvertFromString(colorString) is Color color)
        {
            return new SolidColorBrush(color);
        }

        throw new JsonSerializationException($"Valore colore non valido: {colorString}");
    }
}