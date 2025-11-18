using System.Text.Json;
using System.Text.Json.Serialization;

namespace GiacintFlasher.Lib.Data
{
    public class Config
    {
        internal static JsonSerializerOptions jsonOptions = new() { WriteIndented = true };
        internal const string Version = "V2.0 Lavanda Eagle, Stable";
        internal const string DefaultMessage = "         .   ,\r\n       '. '.  \\  \\\r\n      ._ '-.'. `\\  \\\r\n        '-._; .'; `-.'. \r\n       `~-.; '.       '.\r\n        '--,`           '.\r\n           -='.          ;           %appName%\r\n .--=~~=-,    -.;        ;           %appVersion%\r\n .-=`;    `~,_.;        /            %appAuthor%\r\n`  ,-`'    .-;         |             %appRepo%\r\n   .-~`.    .;         ;\r\n    .;.-   .-;         ,\\\r\n      `.'   ,=;     .-'  `~.-._\r\n       .';   .';  .'      .'   '-.\r\n         .\\  ;  ;        ,.' _  a',\r\n        .'~\";-`   ;      ;\"~` `'-=.)\r\n      .' .'   . _;  ;',  ;\r\n      '-.._`~`.'  \\  ; ; :\r\n           `~'    _'\\\\_ \\\\_ \r\n                 /=`^^=`\"\"/`)-.\r\n                 \\ =  _ =     =\\\r\n                  `\"\"` `~-. =   ;\r\n\r\n";

        //JSON
        [JsonInclude]
        public string MainColor = "\\u001b[38;5;218m\r\n";
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
        [JsonInclude]
        public string CurrentWelcomeMessage = "default.msg";
        [JsonInclude]
        public Dictionary<string, string> AppContexts = new()
        {
            { "%appName%", "Giacint Flasher" },
            { "%appVersion%", Version },
            { "%appAuthor%", "Ykizakyi Zukio" },
            { "%appRepo%", "https://github.com/Ykizakyi-Zukio/GiacintFlasher" },
            { "%appReleases%", "https://github.com/Ykizakyi-Zukio/GiacintFlasher/releases" },
            { "%appPlatform%", Environment.OSVersion.Platform.ToString()}
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
