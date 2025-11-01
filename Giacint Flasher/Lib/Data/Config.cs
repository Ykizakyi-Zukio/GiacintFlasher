using System.Text.Json;
using System.Text.Json.Serialization;

namespace GiacintFlasher.Lib.Data
{
    public class Config
    {
        internal static JsonSerializerOptions jsonOptions = new() { WriteIndented = true };
        internal const string Version = "V1.6 Blue Wolf, Stable (PT Native)";
        [JsonInclude]
        public string MainColor = "\u001b[38;5;75m";
        [JsonInclude]
        public Dictionary<string, string> Links = new Dictionary<string, string>()
        {
            { "platform-tools-latest-windows.zip", "https://dl.google.com/android/repository/platform-tools-latest-windows.zip" },
            { "platform-tools-latest-linux.zip", "https://dl.google.com/android/repository/platform-tools-latest-linux.zip" },
        };


        internal static Config Load()
        {
            if (!File.Exists("config.json"))
                File.WriteAllText("config.json", JsonSerializer.Serialize(new Config(), jsonOptions));
            return JsonSerializer.Deserialize<Config>(File.ReadAllText("config.json"));
        }
        internal string ToJson() => JsonSerializer.Serialize(this, jsonOptions);
    }
}
