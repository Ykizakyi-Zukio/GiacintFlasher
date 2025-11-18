using GiacintFlasher;
using GiacintFlasher.Lib.Data;
using GiacintFlasher.Lib.Services;
using System.Runtime.Serialization.Formatters;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
//using System.Text.Json;
//using System.Diagnostics;
using System.Xml;
using Newtonsoft.Json;

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
        if (!Directory.Exists($"{Environment.CurrentDirectory}\\wms"))
            Directory.CreateDirectory($"{Environment.CurrentDirectory}\\wms");
        if (!File.Exists($"{Environment.CurrentDirectory}\\wms\\default.msg")) 
            File.WriteAllText($"{Environment.CurrentDirectory}\\wms\\default.msg", Config.DefaultMessage);

        string wm;
        if (File.Exists($"{Environment.CurrentDirectory}\\wms\\{Config.CurrentWelcomeMessage}"))
            wm = File.ReadAllText($"{Environment.CurrentDirectory}\\wms\\{Config.CurrentWelcomeMessage}");
        else
            wm = Config.DefaultMessage;

        Console.WriteLine(Config.MainColor);
        Console.Write(StringHelper.ReplaceContexts(Config.AppContexts, Config.DefaultMessage));
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
                Debug.Info($"[DEV MODE] Withount prefix: {JsonConvert.SerializeObject(args.Skip(1).ToList())}");
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
                            case "fd-info":
                                try
                                {
                                    var link = LV.GetPackageInfoAsync(fragArgs[2]).Result;
                                    if (link != null) Debug.Info(JsonConvert.SerializeObject(link));
                                }
                                catch (Exception ex)
                                {
                                    Debug.Error($"Error getting F-Droid link: {ex.Message}");
                                }
                                break;
                            case "fdl":
                            case "fd-latest":
                                if (fragArgs.Length < 3)
                                {
                                    Debug.Warning("No F-Droid package name provided.");
                                    break;
                                }

                                try
                                {
                                    var info = LV.GetPackageInfoAsync(fragArgs[2]).Result;
                                    Package latestPackage = Array.Find(info.Packages, p => p.VersionCode == info.SuggestedVersionCode);
                                    Directory.CreateDirectory($"{Environment.CurrentDirectory}\\packages\\fdroid\\");
                                    LibInstaller.DownloadFileAsync(LV.GetFdroidPackageUrl(info.PackageName, latestPackage.VersionCode), Path.Combine(Environment.CurrentDirectory, $"packages\\fdroid\\{info.PackageName}_{latestPackage.VersionCode}.apk")).Wait();

                                    if (fragArgs.Length == 4 && fragArgs[3] == "--onphone")
                                    {
                                        Debug.Info("Installing package on connected device...");
                                        LibPlus.TryRunLib("adb", $"install -r -g -t \"{Path.Combine(Environment.CurrentDirectory, $"packages\\fdroid\\{info.PackageName}_{latestPackage.VersionCode}.apk")}\"", 5000).Wait();
                                        Debug.Success($"{info.PackageName}_{latestPackage.VersionName}.apk");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.Error($"Error getting F-Droid latest link: {ex.Message}");
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