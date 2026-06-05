using Newtonsoft.Json;

namespace GiacintFlasher.Lib.Data
{
    public class Config
    {
        internal const string Version = "V2.0 Lavanda Eagle, Stable";
        internal const string DefaultMessage = "         .   ,\r\n       '. '.  \\  \\\r\n      ._ '-.'. `\\  \\\r\n        '-._; .'; `-.'. \r\n       `~-.; '.       '.\r\n        '--,`           '.\r\n           -='.          ;           %appName%\r\n .--=~~=-,    -.;        ;           %appVersion%\r\n .-=`;    `~,_.;        /            %appAuthor%\r\n`  ,-`'    .-;         |             %appRepo%\r\n   .-~`.    .;         ;\r\n    .;.-   .-;         ,\\\r\n      `.'   ,=;     .-'  `~.-._\r\n       .';   .';  .'      .'   '-.\r\n         .\\  ;  ;        ,.' _  a',\r\n        .'~\";-`   ;      ;\"~` `'-=.)\r\n      .' .'   . _;  ;',  ;\r\n      '-.._`~`.'  \\  ; ; :\r\n           `~'    _'\\\\_ \\\\_ \r\n                 /=`^^=`\"\"/`)-.\r\n                 \\ =  _ =     =\\\r\n                  `\"\"` `~-. =   ;\r\n\r\n";
        internal static Dictionary<string, string> AppContexts = new()
        {
            { "%appName%", "Giacint Flasher" },
            { "%appVersion%", Version },
            { "%appAuthor%", "Ykizakyi Zukio" },
            { "%appRepo%", "https://github.com/Ykizakyi-Zukio/GiacintFlasher" },
            { "%appReleases%", "https://github.com/Ykizakyi-Zukio/GiacintFlasher/releases" },
            { "%appPlatform%", Environment.OSVersion.Platform.ToString()},
            { "%appUser%", Environment.UserName },
        };

        //JSON
        [JsonRequired]
        public string MainColor = "\u001b[38;5;218m";
        [JsonRequired]
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
        [JsonRequired]
        public Dictionary<string, string> ShortCommands = new()
        {
            { "fb", "fastboot" },
            { "hd", "heimdall"}
        };
        public Source[] LvSources = [];
        [JsonRequired]
        public bool UseLibPlus = true;
        [JsonRequired]
        public bool SmartLibRunner = true;
        [JsonRequired]
        public float WebTimeout = 2.0f;
        [JsonRequired]
        public bool DevMode = false;
        [JsonRequired]
        public bool FullLogging = false;
        [JsonRequired]
        public bool UseBetaFunctions = false;
        [JsonRequired]
        public bool UseLogFile = true;
        [JsonRequired]
        public string CurrentWelcomeMessage = "default.msg";
        [JsonRequired]
        public string RunShortcutInStartup = "";

        internal static Config Load()
        {
            if (!File.Exists(AppContext.BaseDirectory + "\\config.json"))
                File.WriteAllText(AppContext.BaseDirectory + "\\config.json", JsonConvert.SerializeObject(new Config(), Formatting.Indented));
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(AppContext.BaseDirectory + "\\config.json"));
        }
        internal string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
