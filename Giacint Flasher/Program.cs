using GiacintFlasher;
using GiacintFlasher.Lib.Data;
using GiacintFlasher.Lib.Services;
using System.Runtime.Serialization.Formatters;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Text.Json;
//using System.Diagnostics;
using System.Xml;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;

        Flasher.Config = Config.Load();
        Flasher.WelcomeMessage();
        if (LibPlus.FindLib("adb") != null)
        {
            Debug.Info("Starting adb server...");
            _ = Task.Run(() => ProcessHelper.RunCommandAsync(LibPlus.FindLib("adb"), "start-server", false, 10000, true));
        }    

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
        if (LibPlus.FindLib("adb") == null)
            Debug.Warning("ADB library not found. Some commands may not work properly.");
        if (LibPlus.FindLib("fastboot") == null)
            Debug.Warning("Fastboot library not found. Some commands may not work properly.");
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
        try
        {
            string command = input.ToLower().Trim();
            string[] args = command.Split(' ');
            if (args.Length == 0) return;

            if (Config.DevMode && Config.FullLogging)
            {
                Debug.Info($"[DEV MODE] Command received: {command}");
                Debug.Info($"[DEV MODE] Args count: {args.Length}");
                Debug.Info($"[DEV MODE] Args: {string.Join(", ", args)}");
                Debug.Info($"[DEV MODE] Withount prefix: {JsonSerializer.Serialize(args.Skip(1).ToList())}");
            }

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
                    case "sc":
                    case "shortcut":
                        var shortcutsDir = $"{Environment.CurrentDirectory}\\shortcuts";
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

                                if (!Directory.Exists(shortcutsDir)) ;
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
                        //if (Config.UseLibPlus)
                        LibPlus.TryRunLib(fragArgs[1], string.Join(' ', fragArgs.Skip(2).ToArray())).Wait();
                        //else
                        //ProcessHelper.Init(args[1], string.Join(' ', fragArgs.Skip(2).ToArray())).Wait();
                        break;
                    case "lv":
                        if (fragArgs.Length < 2)
                        {
                            Debug.Warning("No Livervorium subcommand provided.");
                            break;
                        }
                        switch (fragArgs[1])
                        {
                            case "i":
                            case "install":
                                if (fragArgs.Length < 3)
                                {
                                    Debug.Warning("No Livervorium package name provided.");
                                    break;
                                }
                                if (Config.LvSources.Length == 0)
                                {
                                    Debug.Error("No Livervorium sources configured. Please add sources in the config file.");
                                    break;
                                }
                                LV.InstallPackage(fragArgs[2]).Wait();
                                break;
                            case "get-redirect":
                                try
                                {
                                    Debug.Info(LV.GetDirectApkLinkAsync(fragArgs[2]).Result);
                                }
                                catch (Exception ex)
                                {
                                    Debug.Error($"Error getting direct link: {ex.Message}");
                                }
                                break;
                            case "fd":
                                try
                                {
                                    var link = LV.GetFdroidJson(fragArgs[2]).Result;
                                    Debug.Info(link);
                                    //LibInstaller.DownloadFileAsync(link, Path.Combine(Environment.CurrentDirectory, fragArgs[2] + ".apk")).Wait();

                                    if (link != null)
                                        Debug.Info(link);
                                    else
                                        Debug.Warning("Package not found on F-Droid.");
                                }
                                catch (Exception ex)
                                {
                                    Debug.Error($"Error getting F-Droid link: {ex.Message}");
                                }
                                break;
                            default:
                                Console.Write("  .---.   ,---.  ,---. \r\n  | ,_|   |   /  |   | \r\n,-./  )   |  |   |  .' \r\n\\  '_ '`) |  | _ |  |       Livervorium Manager CLI Beta\r\n > (_)  ) |  _( )_  |       Install package: lv i --[com.package.name] --..\r\n(  .  .-' \\ (_ o._) /       Params: -mkdir (installing on pc)\r\n `-'`-'|___\\ (_,_) /     From Fdroid: lv fd --[com.package.name]\r\n  |        \\\\     /    \r\n  `--------` `---`     \r\n                        ");
                                break;
                        }
                        break;
                    default:
                        if (Config.SmartLibRunner)
                        {
                            try
                            {
                                fragArgs[0] = Config.ShortCommands[fragArgs[0]] ?? fragArgs[0];
                            }
                            catch { }

                            if (fragArgs.Length < 2)
                            {
                                Debug.Warning("No lib command provided.");
                                break;
                            }
                            LibPlus.TryRunLib(fragArgs[0], string.Join(' ', fragArgs.Skip(1).ToList())).Wait();
                        }
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Error($"Error executing command: {ex.Message}");
            if (Config.UseLogFile)
                File.WriteAllText(Path.Combine(Environment.CurrentDirectory, "error.log"), ex.ToString());
        }
    }
}