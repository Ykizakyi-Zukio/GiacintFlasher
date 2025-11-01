using GiacintFlasher.Lib.Data;
using GiacintFlasher.Lib.Services;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text.Json;
using Debug = GiacintFlasher.Lib.Services.Debug;

namespace GiacintFlasher
{
    internal static class GF
    {
        static string[] libs = new string[]
        {
            "adb.exe",
            "fastboot.exe",
            "make_ext4fs.exe",
            "7z.exe",
            "AdbWinApi.dll",
            "img2simg.exe",
            "simg2img.exe"
        };

        internal static void Command(string[] args)
        {
            try
            {
                switch (args[1])
                {
                    case "h":
                    case "help":

                        break;
                    case "libs":
                        if (!Flasher.Config.UseLibPlus)
                        {
                            Debug.Info("Included libraries, this command provide only standart libraes, u can try Lib+ turn is on in config to view all:");
                            foreach (var lib in libs)
                            {
                                if (File.Exists(Environment.CurrentDirectory + $"\\{lib}"))
                                    Debug.Success($"{lib} WIN");
                                else if (File.Exists(Environment.CurrentDirectory + $"\\{lib.Replace("zip", "")}"))
                                    Debug.Success($"{lib} LINUX");
                                else
                                    Debug.Error($"{lib}");
                            }
                        }
                        else
                        {
                            LibPlus.AllLibs().ToList().ForEach(libPath =>
                            {
                                Debug.Success(Path.GetFileName(libPath));
                            });
                        } 
                        break;
                    case "platform-tools-install":
                    case "pt-i":
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            LibInstaller.DownloadFileAsync(Flasher.Config.Links["platform-tools-latest-windows.zip"], Environment.CurrentDirectory + "\\pt.zip").Wait();
                        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            LibInstaller.DownloadFileAsync(Flasher.Config.Links["platform-tools-latest-linux.zip"], Environment.CurrentDirectory + "\\pt.zip").Wait();
                        else
                        {
                            Debug.Error("Unsupported OS for platform-tools installation.");
                            break;
                        }

                        Debug.Info("Download completed. Extracting package...");
                        ZipFile.ExtractToDirectory(Environment.CurrentDirectory + "\\pt.zip", Environment.CurrentDirectory, true);
                        Debug.Success("Extraction completed. Cleaning up...");

                        foreach (var name in new[] { "adb", "fastboot", "scrcpy" })
                        {
                            foreach (var p in Process.GetProcessesByName(name))
                            {
                                try { p.Kill(); p.WaitForExitAsync().Wait(); } catch { }
                            }
                        }

                        File.Delete(Environment.CurrentDirectory + "\\pt.zip");

                        Debug.Info("Package extracted and deleted");
                        Debug.Success("Platform-tools installed successfully.");

                        break;
                    case "clear":
                    case "c":
                        if (args.Length == 3 && args[2] == "-nwm")
                            Console.Clear();
                        else if (args.Length == 3 && args[2] == "-mini")
                        {
                            Console.Clear();
                            Console.WriteLine(Color.Blue);
                            Console.Write("   |\\---/|\r\n   | ,_, |\r\n    \\_`_/-..----.\r\n ___/ `   ' ,\"\"+ \\  Giacint Flasher\r\n(__...'   __\\    |`.___.';\r\n  (_,...'(_,.`__)/'.....+\r\n\r\n\r\n");
                            break;
                        }
                        else
                        {
                            Console.Clear();
                            Flasher.WelcomeMessage();
                        }
                        break;
                    case "lib-install":
                    case "lib-i":
                        if (args.Length < 3) break;
                        //if (args.Length == 4) { Debug.Error("Incorrect usage of gf lib-i command."); break; }

                        switch (args[3])
                        {
                            case "-unpkg":
                                LibInstaller.DownloadFileAsync(args[2], Environment.CurrentDirectory + "\\lib.zip").Wait();
                                ZipFile.ExtractToDirectory(Environment.CurrentDirectory + "\\lib.zip", Environment.CurrentDirectory, true);
                                break;
                            case "-asname":
                                if (args.Length < 5) { Debug.Error("Please provide a name for the library."); break; }
                                LibInstaller.DownloadFileAsync(args[2], Environment.CurrentDirectory + $"\\{args[5]}").Wait();
                                break;
                        }


                            break;
                    case "config-reset":
                    case "cfg-r":
                        File.WriteAllText("config.json", JsonSerializer.Serialize(new Config(), Config.jsonOptions));
                        Debug.Success("Configuration file reset to default.");
                        break;
                    case "lib-exists":
                        if (args.Length == 3)
                        {
                            string? lib = LibPlus.FindLib(args[2]);
                            if (lib != null)
                                Debug.Success($"Lib found: {lib}");
                        }
                        break;
                    default:
                        Debug.Warning("Unknown command. Use 'gf h' for help.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Error("Error executing gf command: " + ex.Message);
                return;
            }
        }
    }
}
