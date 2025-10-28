using GiacintFlasher.Lib.Services;
using GiacintFlasher.Lib.Data;
using System.Runtime.InteropServices;
using System.IO.Compression;

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
                        Debug.Info("Included libraries:");
                        foreach (var lib in libs)
                        {
                            if (File.Exists(Environment.CurrentDirectory + $"\\{lib}"))
                                Debug.Success($"{lib} WIN");
                            else if (File.Exists(Environment.CurrentDirectory + $"\\{lib.Replace("zip", "")}"))
                                Debug.Success($"{lib} LINUX");
                            else
                                Debug.Error($"{lib}");
                        }
                        break;
                    case "platform-tools-install":
                    case "pt-i":
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            LibInstaller.DownloadFileAsync(LibsLinks.Links["platform-tools-latest-windows.zip"], Environment.CurrentDirectory + "\\pt.zip").Wait();
                        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                            LibInstaller.DownloadFileAsync(LibsLinks.Links["platform-tools-latest-linux.zip"], Environment.CurrentDirectory + "\\pt.zip").Wait();
                        else
                        {
                            Debug.Error("Unsupported OS for platform-tools installation.");
                            break;
                        }

                        Debug.Info("Download completed. Extracting package...");
                        ZipFile.ExtractToDirectory(Environment.CurrentDirectory + "\\pt.zip", Environment.CurrentDirectory, true);
                        Debug.Success("Extraction completed. Cleaning up...");
                        File.Delete(Environment.CurrentDirectory + "\\pt.zip");
                        string[] files = Directory.GetFiles(Environment.CurrentDirectory + "\\platform-tools");

                        Debug.Info("Moving files to current directory...");
                        
                        foreach (var file in files)
                        {
                            Debug.Info($"[\\] Moving {Path.GetFileName(file)}... to {Environment.CurrentDirectory}");
                            string fileName = Path.GetFileName(file);
                            string destPath = Environment.CurrentDirectory + $"\\{fileName}";

                            File.Move(file, destPath, overwrite: true);
                        }
                        Debug.Success("Files moved. Deleting platform-tools directory...");

                        Directory.Delete(Environment.CurrentDirectory + "\\platform-tools", true);

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
                        }
                        else
                        {
                            Console.Clear();
                            Flasher.WelcomeMessage();
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
