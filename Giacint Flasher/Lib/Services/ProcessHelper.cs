using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GiacintFlasher.Lib.Services
{
    internal class ProcessHelper
    {
        internal static async Task<string> RunCommandAsync(string fileName, string arguments)
        {
            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, $"{fileName}.exe")) && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Debug.Error($"{fileName} not installed!");
                return "";
            }

            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                psi.FileName = "/bin/bash";
                psi.Arguments = $"-c \"{fileName} {arguments}\"";
            }

            using var process = new Process { StartInfo = psi };
            process.Start();

            string error = await process.StandardError.ReadToEndAsync();
            if (!string.IsNullOrEmpty(error))
            {
                return "[ERR] " + error;
            }

            string output = await process.StandardOutput.ReadToEndAsync();

            process.WaitForExit();
            int exitCode = process.ExitCode;

            if (exitCode == -1073741510)
                Debug.Error("Command closed by user");

            await process.WaitForExitAsync();

            

            return output.Trim();
        }
        internal static async Task Init(string processName, string[] args) => await Init(processName, string.Join(' ', args));

        internal static async Task Init(string processName, string arg)
        {
            var command = arg.Trim();
            string result = await RunCommandAsync(processName, command);
            Debug.Info(result);
        }
    }
}
