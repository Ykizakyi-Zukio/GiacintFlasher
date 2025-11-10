using System.Text.Json;
using System.Text.Json.Serialization;

namespace GiacintFlasher.Lib.Data
{
    public class Config
    {
        internal static JsonSerializerOptions jsonOptions = new() { WriteIndented = true };
        internal const string Version = "V1.8 Blue Wolf, Stable";

        //JSON
        [JsonInclude]
        public string MainColor = "\u001b[38;5;75m";
        [JsonInclude]
        public Dictionary<string, string> Links = new Dictionary<string, string>()
        {
            { "platform-tools-windows", "https://dl.google.com/android/repository/platform-tools-latest-windows.zip" },
            { "platform-tools-linux", "https://dl.google.com/android/repository/platform-tools-latest-linux.zip" },
            { "heimdall-windows", "https://bitbucket.org/benjamin_dobell/heimdall/downloads/heimdall-suite-1.4.0-win32.zip"},
            { "heimdall-linux", "https://bitbucket.org/benjamin_dobell/heimdall/get/bb448f499c0c.zip" },
            { "oneplus-usb-driver", "https://opfiles.b-cdn.net/OnePlus-USB-Drivers.zip" },
            { "google-usb-driver-windows", "https://dl.google.com/android/repository/usb_driver_r13-windows.zip" },
            { "google-usb-driver-linux", "https://dl.google.com/android/repository/usb_driver_r13-linux.zip" },
        };
        [JsonInclude]
        public Dictionary<string, string> ShortCommands = new()
        {
            { "fb", "fastboot" },
            { "hd", "heimdall"}
        };
        [JsonInclude]
        public Source[] LvSources =
        [
            new Source("APKPURE", "https://d.apkpure.com/b/APK/%com.package.name%?version=latest", true),

        ];
        [JsonInclude]
        public bool UseLibPlus = true;
        [JsonInclude]
        public bool SmartLibRunner = true;
        [JsonInclude]
        public float WebTimeout = 2.0f;
        [JsonInclude]
        public bool DevMode = false;
        [JsonInclude]
        public bool FullLogging = false;
        [JsonInclude]
        public bool UseBetaFunctions = false;
        [JsonInclude]
        public bool UseLogFile = true;


        internal static Config Load()
        {
            if (!File.Exists("config.json"))
                File.WriteAllText("config.json", JsonSerializer.Serialize(new Config(), jsonOptions));
            return JsonSerializer.Deserialize<Config>(File.ReadAllText("config.json"));
        }
        internal string ToJson() => JsonSerializer.Serialize(this, jsonOptions);
    }
}
