using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GiacintFlasher.Lib.Services
{
    internal class ProcessHelper
    {
        internal static async Task<string> RunCommandAsync(string fileName, string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Для Linux/Mac можно явно указать shell
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                psi.FileName = "/bin/bash";
                psi.Arguments = $"-c \"{fileName} {arguments}\"";
            }

            using var process = new Process { StartInfo = psi };
            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            process.WaitForExit();
            int exitCode = process.ExitCode;

            if (exitCode == -1073741510)
                Debug.Error("Command closed by user");

            await process.WaitForExitAsync();

            if (!string.IsNullOrEmpty(error))
                output += "[ERR] " + error;

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
