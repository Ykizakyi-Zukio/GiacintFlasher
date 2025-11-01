using GiacintFlasher;
using GiacintFlasher.Lib.Data;
using GiacintFlasher.Lib.Services;
using System.Runtime.Serialization.Formatters;
using System.Runtime.InteropServices;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

        Flasher.Config = Config.Load();
        Flasher.WelcomeMessage();
        Flasher.Listener();
    }
}

internal static class Flasher
{
    internal static Config Config = new();

    internal static void WelcomeMessage()
    {
        Console.WriteLine(Config.MainColor);
        Console.Write($"                      ,ood8888booo,\r\n                   ,oda8a888a888888bo,\r\n                ,od88888888aa888aa88a8bo,\r\n              ,da8888aaaa88a888aaaa8a8a88b,     Giacint Flasher - Android flashing tool\r\n             ,oa888aaaa8aa8888aaa8aa8a8a88o,    Version: {Config.Version}\r\n            ,88888aaaaaa8aa8888a8aa8aa888a88,   User: {Environment.UserName}\r\n            8888a88aaaaaa8a88aa8888888a888888   Help command: gf h\r\n            888aaaa88aa8aaaa8888; ;8888a88888   OS: {Environment.OSVersion}\r\n            Y888a888a888a8888;'   ;888888a88Y   ADB Help: adb help\r\n             Y8a8aa8a888a88'      ,8aaa8888Y    Fastboot Help: fb help\r\n              Y8a8aa8aa8888;     ;8a8aaa88Y     Github: https://github.com/Ykizakyi-Zukio/GiacintFlasher\r\n               `Y88aa8888;'      ;8aaa88Y'\r\n       ,,;;;;;;;;'''''''         ;8888Y'\r\n    ,,;                         ,888P\r\n   ,;  ,;,                      ;\"\"\r\n  ;       ;          ,    ,    ,;\r\n ;  ;,    ;     ,;;;;;   ;,,,  ;\r\n;  ; ;  ,' ;  ,;      ;  ;   ;  ;\r\n; ;  ; ;  ;  '        ; ,'    ;  ;\r\n`;'  ; ;  '; ;,       ; ;      ; ',\r\n     ;  ;,  ;,;       ;  ;,     ;;;\r\n      ;,,;             ;,,;\r\n\r\n\r\n");
    }

    internal static void Listener()
    {
        while (true)
        {
            var input = Debug.Input();
            if (input == null) continue;

            Command(input);
        }
    }

    internal static void Command(string input)
    {
        
        string command = input.ToLower().Trim();
        string[] args = command.Split(' ');
        if (args.Length == 0) return;

        Span<string> frags = command.Split(">>");
        for (byte i = 0; i < frags.Length; i++)
        {
            frags[i] = frags[i].Trim();
            string[] fragArgs = frags[i].Split(' ');

            switch (fragArgs[0])
            {
                case "gf":
                    if (fragArgs.Length < 2)
                    {
                        Debug.Warning("No subcommand provided. Use 'gf h' for help.");
                        break;
                    }

                    GF.Command(args);
                    break;
                case "fastboot":
                case "fb":
                    if (fragArgs.Length < 2)
                    {
                        Debug.Warning("No fastboot command provided. Use 'fastboot help' for help.");
                        break;
                    }
                    fragArgs[0] = "";
                    ProcessHelper.Init("fastboot", fragArgs).Wait();
                    break;
                case "adb":
                    if (fragArgs.Length < 2)
                    {
                        Debug.Warning("No adb command provided. Use 'adb help' for help.");
                        break;
                    }
                    fragArgs[0] = "";
                    ProcessHelper.Init("adb", fragArgs).Wait();
                    break;
                case "sc":
                case "shortcut":
                    var shortcutsDir = "Environment.CurrentDirectory" + "\\shortcuts\\";
                    if (fragArgs.Length < 2)
                    {
                        Debug.Warning("No shortcut subcommand provided. Use 'sc list' to view shortcuts.");
                        break;
                    }
                    switch (fragArgs[1])
                    {
                        case "create":
                            
                            Console.WriteLine(">> SHORTCUT CREATION COMMAND, SPACE TO EXIT <<");
                            var cmd = Debug.Input();
                            if (String.IsNullOrEmpty(cmd))
                                break;
                            Console.WriteLine(">> SHORTCUT CREATION NAME, SPACE TO EXIT <<");
                            var name = Debug.Input();
                            if (String.IsNullOrEmpty(name))
                                break;

                            if (!Directory.Exists(shortcutsDir));
                                Directory.CreateDirectory(shortcutsDir);
                            Shortcuts.SaveShortcut(name, cmd);
                            break;
                        case "list":
                            if (!Directory.Exists(shortcutsDir))
                                return;
                            Directory.GetFiles(shortcutsDir).ToList().ForEach(file =>
                            {
                                Debug.Info(Path.GetFileNameWithoutExtension(file));
                            });
                            break;
                        default:
                            try
                            {
                                if (fragArgs.Length < 2)
                                {
                                    Debug.Warning("No shortcut name provided.");
                                    break;
                                }
                                Shortcuts.InitShortcut(fragArgs[1].Trim());
                            }
                            catch
                            {   
                                Debug.Error("Shortcut can`t to init.");
                                break;
                            }
                            break;
                    }
                    break;
                case "sh":
                    if (fragArgs.Length < 2)
                    {
                        Debug.Warning("No shell command provided.");
                        break;
                    }
                    var shellName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "C:\\Windows\\System32\\cmd.exe" : "/bin/bash";
                    ProcessHelper.Init(shellName, string.Join(' ', fragArgs.Skip(1).ToArray())).Wait();
                    break;
                case "lib":
                    if (fragArgs.Length < 3)
                    {
                        Debug.Warning("No lib command provided.");
                        break;
                    }
                    ProcessHelper.Init(args[1], string.Join(' ', fragArgs.Skip(2).ToArray())).Wait();
                    break;
            }
        }
    }
}